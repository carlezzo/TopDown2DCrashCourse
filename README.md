# TopDown2DCrashCourse

https://www.youtube.com/watch?v=7iYWpzL9GkM&t=1156s

1. Criar o InventoryManager na Cena

1. Crie um GameObject vazio na cena
1. Renomeie para "InventoryManager"
1. Adicione o script InventoryManager.cs
1. Configure Max Inventory Size (ex: 20 slots)

1. Criar o GameManager na Cena

1. Crie outro GameObject vazio
1. Renomeie para "GameManager"
1. Adicione o script GameManager.cs

1. Criar ScriptableObjects de Itens

1. Clique direito na pasta Assets/Resources/Items/
1. Vá em Create → Inventory → Item
1. Crie itens exemplo:


    - MinerioDeFerro (Material, stackable, max 99)
    - MinerioDeOuro (Material, stackable, max 50)
    - Madeira (Material, stackable, max 99)

4. Criar Prefabs Coletáveis

1. Crie GameObjects com:


    - SpriteRenderer (sprite do item)
    - Collider2D com IsTrigger = true
    - ItemPickup.cs script

2. Configure o campo Item com os ScriptableObjects
3. Salve como prefabs na pasta Assets/Prefabs/Inventory/

4. Configurar Tag do Player

- Certifique-se que o Player tem a tag "Player"
