# ESocial — Documentos e Solução

Repositório de integração com o **eSocial** (ambiente digital unificado do governo federal brasileiro).

## Conteúdo

```
ESocialDocumentos/
├── Documents/                    # Documentação oficial do eSocial
│   ├── CommunicationPackage/     # WSDLs e XSDs (v1.6 — atual, v1.5 — anterior)
│   ├── DeveloperManual/          # Manual do Desenvolvedor (v1.15 — abr/2025)
│   └── TechnicalNotes/           # Leiautes S-1.3 (NT 03.2025) com Tabelas e Regras
│
├── Schemas/                      # XSDs dos eventos para validação local
│   └── 2025-04-22/01_03_00/
│
└── Solution.ESocial/             # Código-fonte da API de integração
```

## Solução

A implementação está em [`Solution.ESocial/`](Solution.ESocial/README.md) — API REST em .NET 10 que abstrai o protocolo SOAP do eSocial, com persistência MySQL e validação XSD local.
