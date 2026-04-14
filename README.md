# Tamagotchi simples (Godot + C#)

Projeto minimo de pet virtual com 3 atributos:
- Fome
- Felicidade
- Energia

## Estrutura
- `Main.tscn`: cena principal com Labels e botoes
- `Main.cs`: controlador da UI e loop de tempo
- `PetStatus.cs`: regras de status do pet

## Como rodar
1. Abra o projeto no Godot 4.5 com suporte a C#.
2. Confirme que a cena inicial esta como `res://Main.tscn`.
3. Execute o projeto (`F5`).

## Comportamento
- A cada 2 segundos, o pet perde status (`Tick`).
- `Alimentar`: aumenta fome, reduz um pouco energia.
- `Brincar`: aumenta felicidade, reduz energia e fome.
- `Dormir`: aumenta energia, reduz fome.
- Se algum status chegar a `0`, o pet desmaia e os botoes sao desativados.
