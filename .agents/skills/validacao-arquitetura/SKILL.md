---
name: validacao-arquitetura
description: Valida e orienta implementações e revisões de arquitetura do backend RUAZ (.NET 10, Clean Architecture). Use ao criar ou modificar entidades, DTOs, controllers, casos de uso, repositórios ou mapeamentos, e antes de concluir alterações no backend. Verifica a separação de um DTO por arquivo, responsabilidades de cada camada, classes de dados sem métodos indevidos e mapeamentos com Mapster.
---

# Validação de arquitetura — RUAZ

## Objetivo

Garantir que alterações no backend RUAZ respeitem Clean Architecture, separação de responsabilidades, organização dos contratos e uso consistente do Mapster. Esta Skill complementa o `AGENTS.md` da raiz do repositório; não substitui seus requisitos de SDD, testes, Git, formatação ou segurança.

## Quando aplicar

- Antes de implementar ou modificar funcionalidades que envolvam o backend.
- Ao criar ou alterar DTOs, entidades, modelos, casos de uso, controllers, interfaces, repositórios e mapeamentos.
- Na revisão final de uma tarefa, incluindo as etapas de revisão previstas no SDD.
- Quando o usuário solicitar revisão, auditoria, validação ou refatoração arquitetural.

Em pedidos de **somente validação**, produza um diagnóstico e sugestões, sem modificar os arquivos. Em pedidos de implementação ou correção, aplique as regras aos arquivos dentro do escopo autorizado.

## 1. Dependências e responsabilidades das camadas

Respeite a organização registrada no `AGENTS.md` e confirme os diretórios reais antes de criar arquivos:

| Camada | Responsabilidade | Restrições |
| --- | --- | --- |
| `API/RUAZ.Domain` | Entidades, tipos, invariantes e regras de negócio | Não depende de `Application`, `Infrastructure`, `API`, EF Core ou Mapster. |
| `API/RUAZ.Application` | Casos de uso, DTOs de aplicação, interfaces e coordenação das regras | Depende de `Domain`; não implementa persistência nem detalhes de serviços externos. |
| `API/RUAZ.Infrastructure` | EF Core, PostgreSQL, migrations, repositórios concretos e integrações externas | Implementa interfaces definidas nas camadas internas; não concentra regras de negócio. |
| `API/RUAZ.API` | Controllers, contratos HTTP específicos, configuração e composição das dependências | Controllers recebem entradas, acionam casos de uso e devolvem respostas. Não executam regras de negócio ou consultas de persistência diretamente. |
| `API/RUAZ.Tests` | Testes unitários e de integração | Testa comportamento por camada, usando xUnit e Moq conforme o projeto. |

- Mantenha as dependências apontadas para dentro; não introduza referência circular.
- Não passe `DbContext`, modelos de persistência ou tipos HTTP para `Domain` ou casos de uso por conveniência.
- Preserve contratos já utilizados e revise consumidores quando houver mudanças deliberadas.
- Coloque validações de entrada no limite apropriado; invariantes de negócio pertencem ao domínio.
- Não crie interfaces, services, factories ou abstrações apenas para aumentar a quantidade de arquivos.

## 2. DTOs: um tipo por arquivo

**Regra obrigatória: cada DTO, Request ou Response deve ter seu próprio arquivo `.cs`.**

- Nomeie o arquivo com o mesmo nome do tipo: `CreateProductRequest.cs`, `ProductResponse.cs`, `LoginRequest.cs`.
- Não declare vários DTOs no mesmo arquivo, mesmo que sejam pequenos, relacionados ou usados apenas por um endpoint.
- Não declare DTOs como classes aninhadas em controllers, services, handlers ou outras classes.
- Separe DTOs de entidades de domínio e de modelos de persistência.
- Organize por funcionalidade e/ou contrato, de acordo com o padrão existente, sem forçar uma reorganização global.
- Requests e Responses podem ter subpastas distintas quando isso melhorar a navegação.
- DTOs são contratos de dados: contenham somente propriedades, sem métodos de negócio, consultas, transformações, mapeamentos ou acesso a serviços.
- Preserve nomes, serialização e formato dos contratos públicos, exceto quando a mudança for parte do pedido.

Exemplo de organização (adapte aos diretórios existentes):

```text
API/RUAZ.Application/
└── DTOs/
    └── Products/
        ├── CreateProductRequest.cs
        ├── UpdateProductRequest.cs
        ├── ProductResponse.cs
        └── ProductSummaryResponse.cs
```

Se os contratos HTTP forem exclusivos da camada `RUAZ.API`, eles podem permanecer nela. A posição do DTO deve respeitar seu consumidor e as referências permitidas entre projetos.

## 3. Classes de dados, métodos e encapsulamento

- **DTOs, Requests, Responses e classes cujo único papel é transportar dados devem conter apenas propriedades.** Não adicione métodos utilitários, lógica de transformação ou regras de negócio a esses tipos.
- **Entidades de domínio não devem receber métodos à toa:** adicione métodos em entidades apenas em situações em que propriedades não possam ser simplesmente atribuídas sem passar por uma validação ou invariante na própria classe (por exemplo, `Price` e `Stock` em `Product`, onde `Stock` também controla `Status`). Propriedades sem invariante podem permanecer como propriedades simples.
- Não adicione métodos genéricos de conveniência a entidades quando sua responsabilidade for mapear, formatar respostas, consultar banco ou chamar APIs externas.
- **Controllers, casos de uso, services, repositórios e outras classes operacionais precisam de métodos** para cumprir sua responsabilidade. A regra de “somente propriedades” não se aplica a elas; seus métodos devem ser coesos e permanecer na camada correta.
- Evite setters públicos em entidades quando eles permitirem quebrar invariantes. Respeite as necessidades reais de EF Core e os padrões já presentes no projeto.
- Não transforme uma entidade rica em modelo anêmico se isso deslocar invariantes relevantes para controllers ou infraestrutura.

## 4. Mapster como padrão de mapeamento

- **Utilize Mapster para converter entidades, modelos e DTOs quando um mapeamento for necessário.** Não introduza AutoMapper nem espalhe atribuições manuais de propriedade pelo código para substituir mapeamentos de rotina.
- Consulte os pacotes e configurações existentes antes de adicionar dependências, registradores ou perfis; reutilize o padrão já adotado no projeto quando compatível com Mapster.
- Use `Adapt<TDestino>()` para mapeamentos simples e configurações `TypeAdapterConfig` para campos com nomes diferentes, relações, transformações ou regras explícitas de mapeamento.
- Centralize configurações de mapeamento em arquivos próprios, com nomes que expressem a origem e o destino ou a funcionalidade; evite configurações volumosas no `Program.cs`.
- Mantenha Mapster fora de `RUAZ.Domain`. Coloque configuração e execução do mapeamento nos limites de `Application`, `API` ou `Infrastructure`, conforme a responsabilidade e as dependências.
- Não use Mapster para executar regras de negócio, validar invariantes ou realizar consultas. Execute a regra na camada apropriada antes ou depois do mapeamento.
- Não mapeie automaticamente campos sensíveis como `PasswordHash`, tokens, segredos ou atributos internos para respostas externas.
- Para alterações em contratos ou configurações não triviais, verifique o mapeamento com testes e revise campos obrigatórios, nulos, coleções e propriedades renomeadas.

Exemplo simples de intenção:

```csharp
var response = product.Adapt<ProductResponse>();
```

O exemplo é ilustrativo: confirme a referência ao Mapster, o local do mapeamento e a existência dos tipos na solução antes de aplicá-lo.

## 5. Fluxo de validação arquitetural

1. Leia `AGENTS.md`, o pedido atual e os arquivos relevantes. Verifique `git status --short`, `git diff` e, quando pertinente, `git diff --cached`. Preserve alterações preexistentes.
2. Identifique os arquivos criados e alterados e as camadas afetadas. Para funcionalidades grandes, siga `docs/sdd/<funcionalidade>/spec.md`, `validation.md` e `tasks.md` conforme o `AGENTS.md`.
3. Confirme que cada classe está na camada adequada e que as dependências entre projetos não estão invertidas.
4. Verifique que cada DTO/Request/Response está em um arquivo próprio, com nome correspondente e sem métodos.
5. Examine entidades e classes de dados: identifique métodos que não encapsulam invariantes ou comportamento de domínio e responsabilidades indevidas.
6. Inspecione conversões entre tipos: confirme o uso do Mapster, a localização da configuração e a proteção de dados sensíveis.
7. Revise controllers, casos de uso e repositórios para encontrar regras, consultas ou integrações alocadas na camada errada.
8. Execute as verificações apropriadas, por exemplo `dotnet build API/RUAZ.slnx` e testes afetados, quando o ambiente permitir. Não declare sucesso para verificações não executadas.
9. Revise o diff final, preserve comentários existentes e **não crie novos comentários, XML docs ou JSDoc** sem pedido explícito. Mantenha a formatação e a indentação do projeto.
10. Se a tarefa usar SDD, registre a verificação no `validation.md`/`tasks.md` conforme o processo já definido. Não marque critérios pendentes como concluídos.

## 6. Critérios objetivos de aprovação

Use esta checklist ao concluir implementações ou revisões:

- [ ] Cada DTO, Request e Response tem seu próprio arquivo, sem tipos de contrato agrupados.
- [ ] Classes de transporte de dados possuem somente propriedades.
- [ ] Métodos de entidades existem apenas quando cumprem comportamento de domínio ou encapsulamento legítimo.
- [ ] Classes operacionais têm métodos limitados à responsabilidade da sua camada.
- [ ] Dependências entre `Domain`, `Application`, `Infrastructure` e `API` seguem a direção correta.
- [ ] Controllers não concentram regras de negócio ou acesso direto à persistência.
- [ ] Implementações de persistência, mensageria e serviços externos ficam na infraestrutura.
- [ ] Mapeamentos necessários usam Mapster e não expõem dados sensíveis indevidamente.
- [ ] Contratos públicos existentes foram preservados ou suas alterações foram tratadas com os consumidores.
- [ ] Testes e build pertinentes foram executados, ou suas limitações foram registradas.
- [ ] O escopo, os comentários existentes, a formatação e o fluxo SDD do `AGENTS.md` foram respeitados.

## 7. Formato da entrega

Apresente, em português brasileiro:

1. **Resultado:** aprovado, aprovado com ressalvas ou reprovado.
2. **Não conformidades:** arquivo/caminho, problema, regra violada e impacto; ordene por gravidade.
3. **Correções:** o que foi ajustado no escopo, ou o que precisa ser alterado se a tarefa foi apenas de auditoria.
4. **Verificações:** comandos/testes executados e resultados; registre explicitamente o que não foi verificado.
5. **Pendências:** decisões ou riscos remanescentes.

Não aprove uma alteração com violações conhecidas das regras obrigatórias. Em revisões pontuais, não refatore arquivos não relacionados sem autorização. Siga o `AGENTS.md` se houver requisitos adicionais ou conflitantes sobre o fluxo de trabalho.
