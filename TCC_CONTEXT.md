# Contexto fixo do TCC

- Projeto: Bichinho Virtual TCC (Godot + C#)
- Objetivo: Trabalho de Conclusão de Curso sobre automação de testes em jogos digitais
- O jogo é laboratório experimental, não produto final
- Sem participação humana → dispensa CEP/FDE
- Ajuda desejada: testes automatizados (GUT), métricas técnicas, documentação e scripts
- Estrutura do jogo: Menu.tscn, Main.cs, PetStatus.cs
- Mecânicas: atributos de fome, energia e felicidade + salvamento offline (pet_save.json)
- Fluxo: menu inicial (novo jogo, continuar, sair), gameplay com ações (alimentar, brincar, dormir), evolução em ciclos de 5 dias
- Persistência: evolução offline calculada pelo tempo decorrido
- Nível de protótipo: versão base estável, sem necessidade de menus completos, arte final ou polimento
- Foco do TCC: aplicar automação de testes, simular passagem de tempo, coletar métricas (bugs, consistência, tempo de execução) e analisar impacto na qualidade
- Estrutura acadêmica: Introdução (problema dos testes manuais), Metodologia (automação aplicada), Resultados (métricas coletadas), Conclusão (impacto da automação)
- Regra fixa: o Copilot **nunca deve executar ações sem autorização explícita do usuário**
