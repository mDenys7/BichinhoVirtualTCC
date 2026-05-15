# Tamagotchi simples (Godot + C#)

Projeto minimo de pet virtual com 3 atributos:
- Fome
- Felicidade
- Energia

## Estrutura
- `Menu.tscn` + `Menu.cs`: cena de menu inicial (Novo Jogo, Continuar, Sair)
- `Game.tscn`: cena principal do jogo com UI de status, sprite e controles
- `Main.cs`: controlador da UI, timer, duracao total, evolucao visual e salvamento
- `PetStatus.cs`: regras de status do pet
- `sprites/pet_day_1.svg`: sprite placeholder (usado em todos os 5 dias por enquanto)

## Como rodar
1. Abra o projeto no Godot 4.5 com suporte a C#.
2. Confirme que a cena inicial esta como `res://Menu.tscn`.
3. Execute o projeto (`F5`).

## Testes unitarios
- Projeto de testes: `tests/BichinhoVirtualTCC.Tests`
- Escopo inicial: regras de negocio de `PetStatus.cs`
- Cobertura atual:
  - estado inicial e reset
  - limites de atributos
  - acoes `Alimentar`, `Brincar` e `Dormir`
  - degradacao por tempo (`Tick` e `ApplyElapsed`)
  - penalidades por fome/energia baixas
  - classificacao final (`Good`, `Neutral`, `Evil`)

Para executar os testes unitarios, rode o projeto de testes isolado:

```powershell
dotnet test ".\tests\BichinhoVirtualTCC.Tests\BichinhoVirtualTCC.Tests.csproj" --nologo
```

Observacao: a suite foi isolada do build do Godot e compila somente `PetStatus.cs`, o que deixa a execucao mais rapida e mais previsivel para medir os resultados do TCC.

## Fluxo do jogo
1. **Menu inicial**: Clique em "Novo Jogo" para iniciar do zero, "Continuar" para carregar partida anterior, ou "Sair" para fechar.
2. **Gameplay**: 
   - A cada 2 segundos, o pet perde status (`Tick`).
   - Use os botoes para `Alimentar`, `Brincar` ou `Dormir`.
   - Se algum status chegar a `0`, o pet desmaia e os botoes sao desativados.
   - Clique em "Novo Ciclo" para comecar um novo ciclo de 5 dias (o save anterior sera perdido).

## Comportamento
- **Duracao**: A partida tem duracao total de 5 dias em tempo real.
- **Evolucao visual**: 
  - A label "Dia: X/5" mostra o dia atual.
  - O sprite muda automaticamente a cada dia (por enquanto, todos usam `pet_day_1.svg` como placeholder).
  - Quando voce desenhar os sprites dias 2, 3, 4, 5, altere os paths em `Main.cs` na array `PetSpritePaths`.
  
- **Acoes**:
  - `Alimentar`: aumenta fome (+20), reduz energia (-4).
  - `Brincar`: aumenta felicidade (+20), reduz energia (-10) e fome (-6).
  - `Dormir`: aumenta energia (+25), reduz fome (-10), aumenta felicidade (+3).

## Evolucao offline
- O jogo salva em `user://pet_save.json`:
  - horario de inicio da partida
  - ultimo horario em que o jogo foi visto
  - status atuais do pet
- Ao abrir novamente, calcula o tempo decorrido e aplica os `Tick`s equivalentes,
  mantendo a evolucao do pet mesmo com o jogo fechado.
- Como o dia e calculado a partir do horario de inicio salvo, o sprite tambem evolui
  corretamente mesmo com o jogo fechado.

## Proximos passos
- [ ] Desenhar pixels art para os dias 2, 3, 4, 5 em `sprites/pet_day_2.svg` ... `sprites/pet_day_5.svg`
- [ ] Adicionar feedback visual (cores, animacoes) para status baixos
- [ ] Adicionar efeitos sonoros
