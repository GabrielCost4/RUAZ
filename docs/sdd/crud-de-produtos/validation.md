# Validação — CRUD de Produtos

| Critério | Descrição | Verificação / Teste Associado | Resultado Esperado |
| --- | --- | --- | --- |
| CA-01 | Usuário (`User`, `Store` ou `Admin`) cria produto válido com `OwnerId` vindo do token e `Stock > 0` | Teste unitário em `ProductTests` (`Create_ValidProduct_SetsOwnerAndAvailableStatus`) | Produto persistido com `OwnerId` do usuário autenticado, `Status = Available` e `ProductResponse` retornado |
| CA-02 | Criação com `Stock == 0` define `Status = Exhausted` automaticamente | Teste unitário em `ProductTests` (`Create_ZeroStock_SetsExhaustedStatus`) | Produto criado com `Status = Exhausted` |
| CA-03 | Criação com dados inválidos (`Title` vazio, `Price < 0`, `Stock < 0` ou `Category` inválida) falha | Teste unitário em `ProductTests` (`Create_InvalidInput_ThrowsBadRequestException`) | Lança `BadRequestException` e não chama `AddAsync` |
| CA-04 | Listagem paginada aplica `page`, `pageSize` e filtros (`category`, `status`) no repositório | Teste unitário em `ProductTests` (`GetAll_NormalizesPaginationAndForwardsFilters`) | Retorna `PagedResult<ProductResponse>` com `TotalCount`, `Page`, `PageSize` e itens mapeados |
| CA-05 | Consulta por `id` existente retorna produto; inexistente lança `NotFoundException` | Testes unitários em `ProductTests` (`GetById_Existing_ReturnsProduct` e `GetById_NotFound_ThrowsNotFoundException`) | Retorna `ProductResponse` ou lança `NotFoundException` |
| CA-06 | Edição pelo próprio dono atualiza campos, `UpdatedAt` e `Status` | Teste unitário em `ProductTests` (`Update_ByOwner_UpdatesProductAndStatus`) | Produto atualizado no repositório com novos valores e `Status` coerente com `Stock` |
| CA-07 | Edição por outro usuário (inclusive `Admin`) em produto alheio é bloqueada | Teste unitário em `ProductTests` (`Update_ByNonOwner_IncludingAdmin_ThrowsForbiddenException`) | Lança `ForbiddenException` e não chama `UpdateAsync` |
| CA-08 | Exclusão pelo dono ou por `Admin` em produto alheio remove o produto | Testes unitários em `ProductTests` (`Delete_ByOwner_DeletesProduct` e `Delete_ByAdminOnOtherUsersProduct_DeletesProduct`) | Chama `DeleteAsync` com o produto encontrado |
| CA-09 | Exclusão por `User` ou `Store` em produto alheio é bloqueada | Teste unitário em `ProductTests` (`Delete_ByNonOwnerNonAdmin_ThrowsForbiddenException`) | Lança `ForbiddenException` e não chama `DeleteAsync` |
| CA-10 | `ProductsController` protege rotas com `[Authorize(Policy = AuthPolicies.User)]` e `GlobalExceptionHandler` mapeia `ForbiddenException` para `403` | Inspeção de código + `dotnet build API/RUAZ.slnx` e `dotnet test API/RUAZ.slnx` | Compilação limpa (0 erros) e todos os testes unitários aprovados |
