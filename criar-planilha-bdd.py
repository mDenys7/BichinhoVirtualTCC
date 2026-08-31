#!/usr/bin/env python3
"""
Criar planilha de testes com cenários em BDD
"""

from openpyxl import Workbook
from openpyxl.styles import Font, PatternFill, Alignment, Border, Side

wb = Workbook()
wb.remove(wb.active)

# Estilos
header_fill = PatternFill(start_color="366092", end_color="366092", fill_type="solid")
header_font = Font(bold=True, color="FFFFFF", size=11)
bdd_fill = PatternFill(start_color="E8F4F8", end_color="E8F4F8", fill_type="solid")
bdd_font = Font(name="Courier New", size=9)
border = Border(
    left=Side(style='thin'),
    right=Side(style='thin'),
    top=Side(style='thin'),
    bottom=Side(style='thin')
)

# =============================================================================
# ABA 1: TESTES MANUAIS COM BDD
# =============================================================================
ws = wb.create_sheet("Testes Manuais - BDD")

# Cenários em BDD
cenarios_bdd = [
    {
        "titulo": "Cenário 1: Estado Inicial",
        "dado": "Que o jogo inicia um novo jogo",
        "quando": "O pet é criado",
        "entao": "O pet deve ter 100 de saúde",
    },
    {
        "titulo": "Cenário 2: Alimentar",
        "dado": "Que o pet tem fome > 0",
        "quando": "Clico em 'Alimentar'",
        "entao": "A fome deve diminuir e respeitar limite (>= 0)",
    },
    {
        "titulo": "Cenário 3: Brincar",
        "dado": "Que o pet está no estado normal",
        "quando": "Clico em 'Brincar'",
        "entao": "Felicidade aumenta E energia diminui, respeitando limites",
    },
    {
        "titulo": "Cenário 4: Dormir",
        "dado": "Que o pet tem energia baixa",
        "quando": "Clico em 'Dormir'",
        "entao": "A energia deve aumentar",
    },
    {
        "titulo": "Cenário 5: Limites dos Atributos",
        "dado": "Que o pet está com valores nos limites",
        "quando": "Alimentar até 100 E deixar degradar até 0",
        "entao": "Valores devem ficar limitados: fome, energia, felicidade entre 0-100",
    },
    {
        "titulo": "Cenário 6: Save/Load",
        "dado": "Que eu tenho um jogo com ações já feitas",
        "quando": "Fecho o jogo E reabrigo",
        "entao": "O estado do pet deve ser restaurado corretamente",
    },
    {
        "titulo": "Cenário 7: Evolução Offline",
        "dado": "Que eu fecho o jogo com o pet vivo",
        "quando": "Espero 30 minutos E reabrigo o jogo",
        "entao": "Os atributos devem ter degradado conforme o tempo",
    },
    {
        "titulo": "Cenário 8: Ciclo Completo (5 dias)",
        "dado": "Que eu estou em um jogo novo",
        "quando": "Jogo até completar 5 dias",
        "entao": "O pet deve aparecer em um estado de evolução consistente",
    },
]

row = 1
for idx, cenario in enumerate(cenarios_bdd, 1):
    # Título
    cell = ws.cell(row=row, column=1)
    cell.value = cenario["titulo"]
    cell.font = Font(bold=True, size=11, color="FFFFFF")
    cell.fill = PatternFill(start_color="1F4E78", end_color="1F4E78", fill_type="solid")
    ws.merge_cells(f"A{row}:F{row}")
    row += 1
    
    # BDD
    bdd_lines = [
        f"Dado {cenario['dado']}",
        f"Quando {cenario['quando']}",
        f"Então {cenario['entao']}"
    ]
    
    for bdd_line in bdd_lines:
        cell = ws.cell(row=row, column=1)
        cell.value = bdd_line
        cell.font = bdd_font
        cell.fill = bdd_fill
        ws.merge_cells(f"A{row}:F{row}")
        row += 1
    
    # Resultado
    resultado_row = row
    ws.cell(row=row, column=1).value = "Tempo (min)"
    ws.cell(row=row, column=2).value = "Resultado"
    ws.cell(row=row, column=3).value = "Dificuldade"
    ws.cell(row=row, column=4).value = "Observação"
    
    for col in range(1, 5):
        cell = ws.cell(row=row, column=col)
        cell.fill = header_fill
        cell.font = header_font
        cell.border = border
    
    row += 1
    
    # Células em branco para preencher
    ws.cell(row=row, column=1).value = ""
    ws.cell(row=row, column=2).value = ""
    ws.cell(row=row, column=3).value = ""
    ws.cell(row=row, column=4).value = ""
    
    for col in range(1, 5):
        cell = ws.cell(row=row, column=col)
        cell.border = border
    
    row += 2  # Espaço entre cenários

# Ajustar largura
ws.column_dimensions['A'].width = 25
ws.column_dimensions['B'].width = 20
ws.column_dimensions['C'].width = 20
ws.column_dimensions['D'].width = 30
ws.column_dimensions['E'].width = 15
ws.column_dimensions['F'].width = 15

# =============================================================================
# ABA 2: RESUMO TESTES MANUAIS
# =============================================================================
ws2 = wb.create_sheet("Resumo Manual")
headers = ["Cenário", "Tempo (min)", "Resultado", "Dificuldade", "Observação"]
for col, header in enumerate(headers, 1):
    cell = ws2.cell(row=1, column=col)
    cell.value = header
    cell.fill = header_fill
    cell.font = header_font
    cell.border = border

cenarios_simples = [
    "Estado Inicial",
    "Alimentar",
    "Brincar",
    "Dormir",
    "Limites",
    "Save/Load",
    "Offline",
    "Ciclo 5 dias",
]

for row_idx, cenario in enumerate(cenarios_simples, 2):
    ws2.cell(row=row_idx, column=1).value = cenario
    for col in range(1, 6):
        ws2.cell(row=row_idx, column=col).border = border

ws2.column_dimensions['A'].width = 20
ws2.column_dimensions['B'].width = 15
ws2.column_dimensions['C'].width = 15
ws2.column_dimensions['D'].width = 15
ws2.column_dimensions['E'].width = 25

# =============================================================================
# ABA 3: TESTES AUTOMATIZADOS
# =============================================================================
ws3 = wb.create_sheet("Testes Automatizados")
headers3 = ["Execução", "Data", "Suíte", "Total", "Passaram", "Falharam", "Tempo (s)"]
for col, header in enumerate(headers3, 1):
    cell = ws3.cell(row=1, column=col)
    cell.value = header
    cell.fill = header_fill
    cell.font = header_font
    cell.border = border

dados = [
    (1, "04/06/2026", "Completa", 30, 30, 0, 2.6),
    (2, "04/06/2026", "Completa", 30, 30, 0, 2.4),
    (3, "04/06/2026", "Regressão", 11, 11, 0, 1.0),
]

for row_idx, (exec, data, suite, total, passou, falhou, tempo) in enumerate(dados, 2):
    ws3.cell(row=row_idx, column=1).value = exec
    ws3.cell(row=row_idx, column=2).value = data
    ws3.cell(row=row_idx, column=3).value = suite
    ws3.cell(row=row_idx, column=4).value = total
    ws3.cell(row=row_idx, column=5).value = passou
    ws3.cell(row=row_idx, column=6).value = falhou
    ws3.cell(row=row_idx, column=7).value = tempo
    
    for col in range(1, 8):
        ws3.cell(row=row_idx, column=col).border = border

for col in range(1, 8):
    ws3.column_dimensions[chr(64 + col)].width = 13

# =============================================================================
# ABA 4: COMPARAÇÃO
# =============================================================================
ws4 = wb.create_sheet("Comparação")
headers4 = ["Critério", "Automatizado", "Manual"]
for col, header in enumerate(headers4, 1):
    cell = ws4.cell(row=1, column=col)
    cell.value = header
    cell.fill = header_fill
    cell.font = header_font
    cell.border = border

criterios = [
    "Tempo total",
    "Repetibilidade",
    "Consistência",
    "Detecção de regressão",
    "Esforço necessário",
    "Aceleração (comparação)",
]

for row_idx, criterio in enumerate(criterios, 2):
    ws4.cell(row=row_idx, column=1).value = criterio
    for col in range(1, 4):
        ws4.cell(row=row_idx, column=col).border = border

ws4.column_dimensions['A'].width = 30
ws4.column_dimensions['B'].width = 20
ws4.column_dimensions['C'].width = 20

# Salvar
output = "COLETA-TESTES-TCC-BDD.xlsx"
wb.save(output)
print(f"✓ Planilha BDD criada: {output}")
print(f"  - Testes Manuais - BDD: cenários em formato Gherkin")
print(f"  - Resumo Manual: tabela rápida para preencher")
print(f"  - Testes Automatizados: com dados já preenchidos")
print(f"  - Comparação: lado a lado")

