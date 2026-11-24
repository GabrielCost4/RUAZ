# RUAZ — Guia do projeto

## Roteiro de trabalho

1. Leia o pedido, confira o estado do Git e classifique a tarefa conforme a seção de SDD.
2. Para uma funcionalidade grande, localize os documentos existentes ou crie `spec.md`, `validation.md` e `tasks.md`, nessa ordem, antes de editar o código.
3. No SDD, execute uma etapa por vez. Verifique o resultado e atualize o `tasks.md` antes de avançar.
4. Revise os arquivos alterados e compare o resultado com o pedido. Corrija divergências e repita as verificações afetadas antes de concluir.

## Propósito

O RUAZ é um projeto de aprendizado que tem como produto um e-commerce funcional, rápido e eficaz. O desenvolvimento deve entregar fluxos utilizáveis e servir para praticar testes unitários, RabbitMQ, código limpo, Clean Architecture e separação de responsabilidades. Priorize regras de negócio explícitas, código fácil de ler e mudanças pequenas que possam ser entendidas e verificadas de ponta a ponta.

O pedido atual e as decisões confirmadas definem o comportamento esperado. Use o código, a configuração e os testes para identificar o comportamento implementado e os contratos vigentes. Se houver divergência, confira o pedido e as decisões registradas. Se uma regra de negócio continuar em aberto e mudar o resultado esperado, esclareça-a antes de implementar o trecho afetado. Para decisões técnicas locais e reversíveis, siga os padrões do repositório e registre a escolha no resumo da tarefa quando ela afetar a continuidade.

## Estrutura da aplicação e responsabilidades das camadas

O RUAZ é composto por um backend em .NET 10 (Clean Architecture), infraestrutura local em Docker e um frontend em Angular 22. Use esta seção como referência das responsabilidades de cada parte e confirme o estado atual diretamente no código antes de iniciar uma tarefa.

| Local | Responsabilidade |
| --- | --- |
| `API/RUAZ.slnx` | Solução que agrupa os projetos do backend em .NET 10. |
| `API/RUAZ.Domain` | Núcleo de negócio: entidades, enums, invariantes e regras de domínio, sem dependências externas. |
| `API/RUAZ.Application` | Coordenação de casos de uso, contratos (DTOs), exceções de aplicação, mapeamentos e interfaces para dependências externas. |
| `API/RUAZ.Infrastructure` | Adaptadores de persistência e serviços externos: EF Core com PostgreSQL, migrations, repositórios concretos, segurança (hash de senha e emissão de JWT) e futuras integrações de mensageria. |
| `API/RUAZ.API` | Camada de entrada HTTP em ASP.NET Core: controllers, políticas de autorização, tratamento global de exceções e composição das dependências. |
| `API/RUAZ.Tests` | Testes automatizados do backend com xUnit e Moq, validando regras de domínio, casos de uso e serviços. |
| `API/InfraDocker` | Ambiente local em Docker Compose para serviços de infraestrutura (como PostgreSQL) e exemplo de configuração. |
| `WEB` | Frontend em Angular 22 com TypeScript e SCSS, responsável pela interface, navegação, estados de tela e consumo da API. |

Consulte os controllers, casos de uso, testes e telas existentes para confirmar quais fluxos já estão disponíveis. RabbitMQ é um objetivo de aprendizado e deve ser introduzido quando um fluxo concreto exigir comunicação assíncrona.

## Arquitetura e implementação

- Mantenha as dependências apontadas para dentro. `Domain` é independente dos outros projetos. `Application` depende de `Domain`. `Infrastructure` implementa contratos de `Application` e pode usar `Domain`. `API` conecta os projetos no ponto de composição em `Program.cs`.
- Coloque invariantes e comportamento de negócio no domínio. Use a aplicação para coordenar casos de uso e definir interfaces de que eles precisam. Mantenha detalhes de EF Core, PostgreSQL, RabbitMQ, JWT e outros serviços externos nos adaptadores correspondentes.
- Use controllers para traduzir requisições HTTP em chamadas a casos de uso e devolver respostas coerentes com os contratos existentes. Preserve o tratamento de erros da API. Use DTOs nos limites externos quando houver um contrato de entrada ou saída.
- No Angular, mantenha os componentes voltados à apresentação e à interação. Centralize chamadas HTTP e seu tratamento compartilhado em serviços quando o fluxo precisar deles. Considere carregamento, sucesso, ausência de dados e falha nas telas em que esses estados existirem.
- Siga a nomenclatura, os padrões e o idioma do trecho alterado. Use nomes que expressem a intenção e abstrações que resolvam uma necessidade concreta. Reaproveite o que já existe e mantenha refatorações no escopo do trabalho.
- Preserve os comentários existentes, inclusive nos arquivos alterados. Adicione comentários novos, incluindo documentação XML e JSDoc, somente quando o usuário pedir explicitamente. Expresse a intenção por meio dos nomes e da estrutura do código.
- Mantenha todo código novo ou alterado corretamente indentado. No backend C#, siga a indentação aplicada pelo `dotnet format` com a configuração da solução. Limite a formatação aos arquivos alterados. Nos demais arquivos, siga a configuração do projeto ou, na falta dela, o padrão predominante do código adjacente. Revise a indentação no diff antes de concluir.
- Preserve contratos usados por outros fluxos, salvo mudança deliberada. Ao alterar um contrato HTTP, um esquema persistido ou uma mensagem, revise os consumidores afetados. Mudanças no modelo persistido devem incluir a migration correspondente quando necessária.
- Trate desempenho a partir do fluxo real. Em listas que podem crescer, considere paginação e filtros no banco. Investigue gargalos com medidas antes de introduzir cache, processamento assíncrono ou outras otimizações.
- Mantenha senhas, chaves JWT e credenciais de banco ou do broker fora do código, dos logs e do controle de versão. Use configuração local ou variáveis de ambiente e valores fictícios nos exemplos. Valide entradas e permissões na API de acordo com o caso de uso.

## SDD para funcionalidades grandes

Codex e Antigravity devem seguir o desenvolvimento orientado por especificação (Spec-Driven Development, SDD) em toda funcionalidade grande. Aplique SDD quando o pedido introduzir um fluxo de negócio, como carrinho ou checkout, ou exigir mudanças coordenadas no comportamento de contratos, persistência ou integrações. Uma correção localizada que restaura uma regra existente e preserva os contratos segue o fluxo normal de implementação e testes, mesmo que ajuste mais de uma camada.

Crie uma pasta por funcionalidade, com nome em minúsculas e palavras separadas por hífens, como `carrinho-de-compras`.

```text
docs/
└── sdd/
    └── <funcionalidade>/
        ├── spec.md
        ├── validation.md
        └── tasks.md
```

1. **Crie o `spec.md` antes de implementar.** Leia a solicitação e os arquivos relevantes. Registre o objetivo, o escopo, as regras de negócio, os fluxos, os contratos afetados e as dúvidas em aberto. Fundamente as decisões no pedido e no repositório. Resolva dúvidas que alterem o comportamento esperado antes de implementar o trecho afetado.
2. **Crie o `validation.md` a partir da especificação.** Identifique os critérios de aceitação, por exemplo `CA-01`, e associe cada um a um teste ou a um procedimento de verificação com resultado esperado. Inclua os cenários de sucesso, de falha e de limite relevantes.
3. **Crie o `tasks.md` antes de editar o código.** Divida a implementação em etapas pequenas e ordenadas. Registre cada etapa como `- [ ]` e indique os critérios de aceitação correspondentes. Inclua um item para a revisão final.
4. **Implemente, verifique e marque uma etapa por vez.** Releia os trechos correspondentes do `spec.md` e do `validation.md`. Faça a mudança, revise o diff e execute as verificações aplicáveis. Marque o item como `- [x]` assim que a etapa estiver concluída e verificada. Junto ao item, registre o identificador do critério, o comando ou procedimento executado e o resultado observado. Só então comece a próxima etapa. Se uma verificação falhar, corrija e verifique novamente. Se houver um bloqueio, deixe o item pendente e registre o que foi feito, a verificação pendente e o motivo. Resolva o bloqueio antes de iniciar a próxima etapa.
5. **Faça a revisão final antes de encerrar.** Releia `spec.md`, `validation.md`, `tasks.md` e os arquivos criados ou alterados. Compare o diff com o pedido atual e as decisões confirmadas. Se houver lacunas, corrija, execute novamente as verificações afetadas, atualize o `tasks.md` e repita a revisão. Marque a revisão final como `- [x]` e encerre a funcionalidade somente quando todas as etapas anteriores estiverem concluídas e todos os critérios aplicáveis tiverem evidência registrada.

## Testes e verificação

- Teste regras e casos de uso novos ou alterados com cenários representativos. Inclua falhas e limites quando fizerem parte do comportamento. Reproduza regressões com um teste quando isso for viável.
- Escreva testes unitários determinísticos, com nomes que descrevam o cenário e o resultado esperado. Verifique o comportamento observável. Use mocks para dependências externas quando necessário. Testes unitários devem rodar sem PostgreSQL, RabbitMQ ou rede.
- Use testes de integração quando a verificação depender de consultas, migrations, HTTP, mensageria ou configuração entre componentes. Separe esses cenários dos testes unitários e documente seus requisitos locais quando forem introduzidos.
- Rode as verificações que correspondem à mudança. Se uma verificação falhar ou ficar pendente, informe o comando, o resultado e o motivo conhecido. Registre como verificado apenas o que tiver sido executado ou inspecionado.

## Uso de RabbitMQ

- Introduza mensageria quando um fluxo concreto precisar de comunicação assíncrona. Identifique produtor, consumidor, finalidade da mensagem e comportamento esperado em caso de falha.
- Mantenha o código de conexão, publicação e consumo na infraestrutura. Se o caso de uso precisar publicar uma mensagem, defina o contrato no limite da aplicação. Trate o formato da mensagem como um contrato entre produtor e consumidor.
- Havendo possibilidade de reentrega, projete o consumidor para tolerar duplicatas. Confirme a mensagem após concluir com sucesso o processamento que precisa ser durável. Defina tentativas limitadas e o destino de mensagens que não puderem ser processadas.
- Se um fluxo atualizar o banco e publicar uma mensagem, trate explicitamente o risco de uma operação concluir sem a outra. Avalie outbox quando a consistência do fluxo exigir essa garantia. Cubra as regras com testes unitários e valide com o broker os comportamentos que dependam dele.

## Comandos locais

A partir da raiz, com o SDK .NET 10 instalado:

```powershell
dotnet build API/RUAZ.slnx
dotnet test API/RUAZ.slnx
dotnet run --project API/RUAZ.API/RUAZ.API.csproj
```

Para operações locais que dependem de PostgreSQL, use `API/InfraDocker/.env.example` como base para o `.env` local no mesmo diretório e execute `docker compose up -d` em `API/InfraDocker`. Configure `ConnectionStrings__DefaultConnection` para apontar para a instância local e confira os requisitos de JWT na configuração da API. Verifique a necessidade de aplicar migrations antes de exercitar um fluxo que usa o banco.

Para o frontend, em um terminal separado a partir da raiz, com Node.js e npm compatíveis com o projeto:

```powershell
Set-Location WEB
npm ci
npm run build
npm test -- --watch=false
npm start
```

Use `npm ci` para preparar as dependências quando necessário e `npm start` quando quiser executar o servidor de desenvolvimento. Confira `WEB/package.json`, `WEB/angular.json` e os perfis de execução da API para detalhes atuais.

## Continuidade entre Codex e Antigravity

Use este `AGENTS.md` como referência compartilhada. Se a ferramenta da sessão precisar de contexto explícito, indique a leitura deste arquivo antes das alterações. Registre aqui decisões duradouras que afetam trabalhos futuros e mantenha os detalhes de uma tarefa no próprio diff e no resumo da entrega.

1. Ao começar ou retomar, confira `git status --short`, `git diff` e `git diff --cached` quando houver mudanças em andamento. Leia os arquivos e testes relevantes. Use o estado do repositório para confirmar o que já foi feito, inclusive ao continuar trabalho iniciado em outra ferramenta.
2. Preserve alterações locais já existentes. Se houver sobreposição com a tarefa, entenda a intenção pelo diff antes de editar. Evite sobrescrever mudanças alheias ao escopo ou descartar trabalho local.
3. Ao retomar uma funcionalidade em SDD, leia `spec.md`, `validation.md` e `tasks.md` na pasta da funcionalidade. Compare as evidências registradas com o código e o diff atuais. Se uma alteração invalidar a evidência de um item concluído, volte o item para `- [ ]` e registre o motivo. Retome o primeiro item pendente na ordem do `tasks.md`.
4. Ao entregar, resuma as alterações, as decisões relevantes e as verificações executadas com seus resultados. Se o trabalho ficar interrompido, indique o que já está no workspace, o que falta e o próximo passo concreto.
