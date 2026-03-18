param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string]$ApiUrl = "http://localhost:5167",
    [string]$JaegerUrl = "http://localhost:16686",
    [string]$ChromePath = "C:\Program Files\Google\Chrome\Application\chrome.exe",
    [string]$ScreenshotRelativePath = "docs\assets\jaeger-trace.png"
)

$ErrorActionPreference = "Stop"

$sdlUrl = "$ApiUrl/graphql?sdl"
$graphqlUrl = "$ApiUrl/graphql"
$jaegerApiUrl = "$JaegerUrl/api/traces?service=HotChocolateDddCqrsTemplate&limit=20&lookback=1h"
$screenshotPath = Join-Path $RepositoryRoot $ScreenshotRelativePath
$artifactsDir = Join-Path $RepositoryRoot "artifacts"
$stdoutLog = Join-Path $artifactsDir "jaeger-api.stdout.log"
$stderrLog = Join-Path $artifactsDir "jaeger-api.stderr.log"
$apiProcess = $null

function Wait-UntilHealthy {
    param(
        [string]$ContainerName,
        [int]$Attempts = 60
    )

    for ($attempt = 0; $attempt -lt $Attempts; $attempt++) {
        $health = docker inspect -f "{{.State.Health.Status}}" $ContainerName 2>$null

        if ($health -eq "healthy") {
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "Container '$ContainerName' did not become healthy."
}

function Wait-ForHttpOk {
    param(
        [string]$Url,
        [int]$Attempts = 90
    )

    for ($attempt = 0; $attempt -lt $Attempts; $attempt++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5

            if ($response.StatusCode -eq 200) {
                return
            }
        }
        catch {
        }

        Start-Sleep -Seconds 2
    }

    $stdout = if (Test-Path $stdoutLog) { Get-Content $stdoutLog -Raw } else { "" }
    $stderr = if (Test-Path $stderrLog) { Get-Content $stderrLog -Raw } else { "" }
    throw "API did not become ready at $Url.`nStdout:`n$stdout`nStderr:`n$stderr"
}

function Get-TraceId {
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        $response = Invoke-RestMethod -Uri $jaegerApiUrl -TimeoutSec 15

        $match = $response.data |
            Sort-Object {
                ($_.spans | Measure-Object -Property startTime -Maximum).Maximum
            } -Descending |
            Where-Object {
                ($_.spans | ForEach-Object { $_.operationName }) -contains "graphql.mutation.createProduct"
            } |
            Select-Object -First 1

        if ($match) {
            return $match.traceID
        }

        Start-Sleep -Seconds 2
    }

    $stdout = if (Test-Path $stdoutLog) { Get-Content $stdoutLog -Raw } else { "" }
    $stderr = if (Test-Path $stderrLog) { Get-Content $stderrLog -Raw } else { "" }
    throw "No Jaeger trace containing graphql.mutation.createProduct was found.`nStdout:`n$stdout`nStderr:`n$stderr"
}

function Invoke-GraphQlMutation {
    param([string]$CategoryId)

    $mutation = @'
mutation($input: CreateProductInput!) {
  createProduct(input: $input) {
    product {
      id
      name
    }
    errors {
      code
      message
    }
  }
}
'@

    $payload = @{
        query = $mutation
        variables = @{
            input = @{
                name = "Trace Product"
                price = 299.0
                currency = "USD"
                sku = "TRACE-$([int](Get-Random -Minimum 1000 -Maximum 9999))"
                categoryId = $CategoryId
            }
        }
    } | ConvertTo-Json -Depth 10

    $response = Invoke-RestMethod -Method Post -Uri $graphqlUrl -ContentType "application/json" -Body $payload -TimeoutSec 30

    if ($response.errors) {
        throw "GraphQL returned transport errors: $($response.errors | ConvertTo-Json -Depth 10)"
    }

    if ($response.data.createProduct.errors.Count -gt 0) {
        throw "GraphQL returned domain errors: $($response.data.createProduct.errors | ConvertTo-Json -Depth 10)"
    }
}

function Ensure-Category {
    $categoryId = [Guid]::NewGuid().ToString()
    $sqlFile = Join-Path $artifactsDir "seed-category.sql"
    $sql = @"
insert into categories ("Id", "Name", "Slug")
values ('$categoryId', 'Telemetry', 'telemetry')
on conflict do nothing;
"@

    Set-Content -Path $sqlFile -Value $sql -NoNewline
    Get-Content $sqlFile | docker exec -i hc-ddd-template-postgres psql -U postgres -d hotchocolate_template | Out-Host

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to insert a seed category for the Jaeger trace capture."
    }

    return $categoryId
}

try {
    if (-not (Test-Path $ChromePath)) {
        throw "Chrome executable was not found at '$ChromePath'."
    }

    New-Item -ItemType Directory -Force -Path $artifactsDir | Out-Null
    New-Item -ItemType Directory -Force -Path (Split-Path $screenshotPath -Parent) | Out-Null

    docker compose down -v | Out-Host
    docker compose up -d | Out-Host

    Wait-UntilHealthy -ContainerName "hc-ddd-template-postgres"

    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:ASPNETCORE_URLS = $ApiUrl

    $apiProcess = Start-Process dotnet `
        -ArgumentList "run", "--project", "src/Api", "--no-launch-profile" `
        -WorkingDirectory $RepositoryRoot `
        -RedirectStandardOutput $stdoutLog `
        -RedirectStandardError $stderrLog `
        -PassThru

    Wait-ForHttpOk -Url $sdlUrl

    $categoryId = Ensure-Category
    Invoke-GraphQlMutation -CategoryId $categoryId

    Start-Sleep -Seconds 8

    $traceId = Get-TraceId

    Remove-Item $screenshotPath -ErrorAction SilentlyContinue

    & $ChromePath `
        "--headless=new" `
        "--disable-gpu" `
        "--window-size=1600,1200" `
        "--virtual-time-budget=10000" `
        "--screenshot=$screenshotPath" `
        "$JaegerUrl/trace/$traceId" | Out-Host

    if (-not (Test-Path $screenshotPath)) {
        throw "Chrome did not produce the Jaeger screenshot."
    }

    Write-Host "Captured Jaeger trace screenshot at $screenshotPath"
}
finally {
    if ($apiProcess -and -not $apiProcess.HasExited) {
        Stop-Process -Id $apiProcess.Id -Force
    }

    docker compose down -v | Out-Host
}
