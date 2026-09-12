# QueryRe 

QueryRe lets a user ask a Microsoft SQL Server database a question in English. A local Ollama model changes the question into a SELECT query. The application checks the SQL, runs it with a read-only SQL Server login, and shows the result in a table.

This edition is intentionally small enough for a student to understand and explain in a viva.

## Simple architecture

```text
QueryRe.Web   → pages, QueryService, OllamaService
      ↓
QueryRe.Data  → SqlService, HistoryService
      ↓
QueryRe.Core  → models and SqlValidator

QueryRe.Tests → simple SqlValidator tests
```
