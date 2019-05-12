import os, json
with open('questions.que', 'r', encoding='utf-8') as file:
    ctx = file.read()
    data = json.loads(ctx)
print(data)
with open('questions.que.bak', 'w', encoding='utf-8') as file:
    ctx = json.dumps(data, ensure_ascii=False)
    file.write(ctx)
