# Especificação — CRUD de Produtos

## 1. Objetivo

Implementar o fluxo completo de CRUD de produtos na API RUAZ seguindo Clean Architecture, utilizando as policies e claims de `TypeActivity` configuradas no JWT para controle de acesso granular por perfil (`User`, `Store` e `Admin`) e por posse (`OwnerId`).

## 2. Escopo

- **Domínio (`RUAZ.Domain`):** regras de consistência da entidade `Product` (validação de preço, estoque e atualização automática de `StatusProduct` quando `Stock == 0` vs `Stock > 0`, além de `UpdatedAt`).
- **Aplicação (`RUAZ.Application`):**
  - Contratos (DTOs): `CreateProductRequest`, `UpdateProductRequest`, `ProductResponse`, `ProductFilterRequest`, `PagedResult<T>`.
  - Interface de repositório: `IProductRepository`.
  - Caso de uso: `ProductService`.
  - Exceção: `ForbiddenException` (para tentativas de alteração/exclusão sem posse/permissão).
- **Infraestrutura (`RUAZ.Infrastructure`):**
  - `ProductRepository` com EF Core e PostgreSQL (paginação e filtros no banco).
- **API (`RUAZ.API`):**
  - `ProductsController` (`/api/products`) protegido por `[Authorize(Policy = AuthPolicies.User)]`.
  - Mapeamento de `ForbiddenException` para HTTP `403 Forbidden` no `GlobalExceptionHandler`.
  - Registro das dependências no `Program.cs`.
- **Testes (`RUAZ.Tests`):**
  - Testes unitários determinísticos cobrindo todas as regras do `ProductService` e invariantes de `Product`.

Fora do escopo:
- Endpoints exclusivos de funcionalidades futuras de `Store` (ex.: dashboard/relatórios de loja).
- Integração com RabbitMQ ou frontend Angular (`WEB/`).

## 3. Regras de Negócio

1. **Autenticação e Policy base:**
   - Todos os endpoints de `/api/products` exigem usuário autenticado com a policy `AuthPolicies.User` (aceita `User`, `Store` e `Admin`).
   - O `OwnerId` e o `TypeActivity` do usuário autenticado são extraídos das claims do JWT (`sub` / `ClaimTypes.NameIdentifier` e `ClaimTypes.Role` / `type_activity`).

2. **Criação (`POST /api/products`):**
   - `User`, `Store` e `Admin` podem criar produtos.
   - `OwnerId` é sempre atribuído a partir do usuário autenticado (nunca aceito do corpo da requisição).
   - `Title` é obrigatório (não vazio/whitespace); `Price` deve ser `>= 0`; `Stock` deve ser `>= 0`; `Category` deve ser um valor válido de `ProductsCategory`. Caso contrário, lança `BadRequestException`.
   - `Image` é opcional (se nula/omitida, assume `[]`).
   - `Status` inicial: `StatusProduct.Available` quando `Stock > 0`, ou `StatusProduct.Exhausted` quando `Stock == 0`.
   - `CreatedAt` preenchido com `DateTime.UtcNow`.

3. **Leitura (`GET /api/products` e `GET /api/products/{id}`):**
   - `User`, `Store` e `Admin` podem listar e consultar qualquer produto.
   - `GET /api/products` suporta paginação (`page >= 1`, padrão `1`; `pageSize` entre `1` e `100`, padrão `10`) e filtros opcionais por `category` (`ProductsCategory?`) e `status` (`StatusProduct?`) executados no banco.
   - `GET /api/products/{id}` retorna `NotFoundException` (`404`) se o produto não existir.

4. **Edição (`PUT /api/products/{id}`):**
   - Apenas o dono do produto (`product.OwnerId == currentUserId`) pode editá-lo.
   - Nem mesmo `Admin` pode editar produto alheio; qualquer usuário diferente do dono recebe `ForbiddenException` (`403`).
   - Se o produto não existir, lança `NotFoundException` (`404`).
   - Aplica as mesmas validações de `Title`, `Price`, `Stock` e `Category`. Se `Image` não for enviada (`null`), preserva a imagem atual.
   - Atualiza `Status` conforme `Stock` (`> 0` -> `Available`, `== 0` -> `Exhausted`) e `UpdatedAt = DateTime.UtcNow`.

5. **Exclusão (`DELETE /api/products/{id}`):**
   - O dono do produto (`product.OwnerId == currentUserId`) pode excluí-lo.
   - `Admin` (`TypeActivity.Admin`) também pode excluir produtos de terceiros.
   - `User` ou `Store` tentando excluir produto de outro usuário recebe `ForbiddenException` (`403`).
   - Se o produto não existir, lança `NotFoundException` (`404`).

## 4. Contratos HTTP

- `GET /api/products?page=1&pageSize=10&category=&status=` -> `200 OK` (`PagedResult<ProductResponse>`)
- `GET /api/products/{id}` -> `200 OK` (`ProductResponse`) | `404 Not Found`
- `POST /api/products` -> `201 Created` (`ProductResponse` com header `Location`) | `400 Bad Request`
- `PUT /api/products/{id}` -> `200 OK` (`ProductResponse`) | `400 Bad Request` | `403 Forbidden` | `404 Not Found`
- `DELETE /api/products/{id}` -> `204 No Content` | `403 Forbidden` | `404 Not Found`
