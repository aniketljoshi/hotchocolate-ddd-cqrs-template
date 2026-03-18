# Seed Data Reference

The local Compose stack seeds the following catalog records when `SampleData:Enabled` is `true` and the database is empty.

## Categories

| Name | Slug | Id |
| --- | --- | --- |
| Books | `books` | `820b63b4-ec53-4f06-9871-57d9bf14bb51` |
| Courses | `courses` | `d8ee54d4-cce8-4948-a0ee-ebea85a34895` |

## Products

| Name | Sku | Category | Id |
| --- | --- | --- | --- |
| Domain-Driven Design Distilled | `BOOK-001` | Books | `4f2bde1d-0f65-488d-bca5-6efc5dd55b0d` |
| HotChocolate in Practice | `COURSE-001` | Courses | `c8d8a522-67f5-4ee6-8bf0-05a6ee1c7f21` |
| CQRS Starter Kit | `COURSE-002` | Courses | `16a5d6e8-d711-4722-a968-c328a8f1fa84` |
| Clean Architecture Notebook | `BOOK-002` | Books | `cf8c6717-01ee-4df6-a1e4-c1f41063f636` |

## Request mapping

- `Create Product` in Insomnia uses the seeded Books category ID by default.
- `Update Product Price` in Insomnia uses the seeded Domain-Driven Design product ID by default.
- `Get Product By Id` also points at the same seeded product so the read path works before any mutations are run.
