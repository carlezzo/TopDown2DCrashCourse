# Guia de Configuração do Sistema de Armas no Unity Editor

## ✅ Já Implementado (Via Código)

- ✅ Scripts core: IWeapon, WeaponBase, WeaponData, WeaponManager
- ✅ StaffWeapon implementation
- ✅ WizardController modificado para usar WeaponManager
- ✅ Estrutura de pastas criada

## 📋 Passos a Seguir no Unity Editor

### **FASE 1: Preparar Sprites do Staff**

#### 1.1 Verificar Importação dos Aseprite Files

1. No Unity, vá para `Assets/Art/Magic/Characters/wizard/Wapon/`
2. Verifique os 3 arquivos Aseprite:
   - `wizard_iddle_staff.aseprite`
   - `wizard_walk_staff.aseprite`
   - `wizard_atack_staff.aseprite`
3. Clique em cada arquivo e no Inspector verifique:
   - **Sprite Mode**: Multiple
   - **Pixels Per Unit**: 16
   - **Generate Animation Clips**: ✅ (deve estar marcado)
4. Se necessário, clique em "Apply" para reimportar

#### 1.2 Localizar os Sprites Extraídos

1. Expanda cada arquivo .aseprite no Project window (clique na setinha)
2. Você deve ver múltiplos sprites extraídos:
   - **wizard_iddle_staff**: sprites idle (front, back, side)
   - **wizard_walk_staff**: sprites walk (front, back, side)
   - **wizard_atack_staff**: sprites attack (wizard_atack_staff_0 até _10)

---

### **FASE 2: Criar Animation Clips do Staff**

#### 2.1 Criar Idle Animations (3 clips)

**Para cada direção (front, back, side):**

1. No Project, vá para `Assets/Characters/Wizard/Animations/Staff/`
2. Clique com botão direito → **Create → Animation**
3. Nomeie: `staff_idle_front.anim`

**Configurar a animação:**
1. Com o arquivo .anim selecionado, abra a **Animation Window** (Window → Animation → Animation)
2. Arraste os sprites idle do staff correspondentes para a timeline:
   - Para `staff_idle_front`: arraste sprites idle front da direção frontal
   - Para `staff_idle_back`: arraste sprites idle back
   - Para `staff_idle_side`: arraste sprites idle side
3. Configure:
   - **Sample Rate**: 6 (frames por segundo)
   - **Loop**: ✅ marcado

**Repita para as 3 direções:**
- ✅ `staff_idle_front.anim`
- ✅ `staff_idle_back.anim`
- ✅ `staff_idle_side.anim`

#### 2.2 Criar Walk Animations (3 clips)

**Mesma técnica, mas usando sprites walk:**

1. Criar `staff_walk_front.anim`
2. Criar `staff_walk_back.anim`
3. Criar `staff_walk_side.anim`
4. Arraste sprites walk correspondentes
5. Sample Rate: 6, Loop: ✅

#### 2.3 Criar Attack Animations (3 clips)

**IMPORTANTE: Usar sprites de wizard_atack_staff.aseprite**

1. Criar `staff_attack_front.anim`
2. Criar `staff_attack_back.anim`
3. Criar `staff_attack_side.anim`
4. Arraste sprites attack correspondentes (frames 0-10)
5. Sample Rate: 6
6. **Loop: ❌ DESMARCADO** (attack toca uma vez só)

**Você terá 9 animation clips ao final desta fase.**

---

### **FASE 3: Criar Animator Controller do Staff**

#### 3.1 Criar o Controller

1. Vá para `Assets/Characters/Wizard/Animations/`
2. Clique direito → **Create → Animator Controller**
3. Nome: `AC_Staff`

#### 3.2 Configurar Parâmetros

1. Abra o Animator (duplo clique em AC_Staff)
2. Na aba **Parameters**, adicione:
   - `moveX` (Float) - default: 0
   - `moveY` (Float) - default: 0
   - `isMoving` (Bool) - default: false
   - `attack` (Trigger)

#### 3.3 Criar Idle Blend Tree

1. No Animator, clique direito → **Create State → From New Blend Tree**
2. Renomeie para: `Idle_BlendTree`
3. Duplo clique no estado para entrar
4. Selecione o Blend Tree, no Inspector:
   - **Blend Type**: 2D Freeform Directional
   - **Blend Parameter X**: moveX
   - **Blend Parameter Y**: moveY

5. Clique em **+** para adicionar Motion Fields (3 vezes)
6. Arraste as animações idle:
   - **staff_idle_front** → Position X: 0, Y: -1
   - **staff_idle_back** → Position X: 0, Y: 1
   - **staff_idle_side** → Position X: 1, Y: 0

7. Clique em **+** novamente e adicione:
   - **staff_idle_side** → Position X: -1, Y: 0

8. Volte para Base Layer (clique em "Base Layer" no topo)

#### 3.4 Criar Walk Blend Tree

1. Clique direito → **Create State → From New Blend Tree**
2. Renomeie: `Walk_BlendTree`
3. Mesma configuração que Idle, mas usando:
   - `staff_walk_front` em (0, -1)
   - `staff_walk_back` em (0, 1)
   - `staff_walk_side` em (1, 0) e (-1, 0)

#### 3.5 Criar Attack Blend Tree

1. Clique direito → **Create State → From New Blend Tree**
2. Renomeie: `Attack_BlendTree`
3. Mesma configuração, mas usando:
   - `staff_attack_front` em (0, -1)
   - `staff_attack_back` em (0, 1)
   - `staff_attack_side` em (1, 0) e (-1, 0)

**IMPORTANTE:**
- Selecione o estado `Attack_BlendTree`
- No Inspector, **DESMARQUE** "Loop Time"

#### 3.6 Criar Transições

**Idle ↔ Walk:**
1. Clique direito em `Idle_BlendTree` → **Make Transition** → clique em `Walk_BlendTree`
2. Selecione a transição (setinha), no Inspector:
   - **Has Exit Time**: ❌ desmarcado
   - **Conditions**: `isMoving` = true

3. Repita ao contrário (Walk → Idle):
   - **Conditions**: `isMoving` = false

**Any State → Attack:**
1. Clique direito em **Any State** → **Make Transition** → clique em `Attack_BlendTree`
2. Na transição:
   - **Has Exit Time**: ❌ desmarcado
   - **Conditions**: `attack` (trigger)

**Attack → Idle:**
1. Clique direito em `Attack_BlendTree` → **Make Transition** → clique em `Idle_BlendTree`
2. Na transição:
   - **Has Exit Time**: ✅ MARCADO
   - **Exit Time**: 1.0
   - **Transition Duration**: 0.1

#### 3.7 Definir Estado Default

1. Clique direito em `Idle_BlendTree` → **Set as Layer Default State**
2. O estado deve ficar laranja

---

### **FASE 4: Criar StaffWeapon Prefab**

#### 4.1 Criar GameObject Base

1. Na Hierarchy, clique direito → **Create Empty**
2. Renomeie: `StaffWeapon`
3. Transform → Position: (0.15, 0, 0)

#### 4.2 Adicionar Componentes

**SpriteRenderer:**
1. Add Component → **Sprite Renderer**
2. **Sprite**: arraste qualquer sprite de staff (para preview)
3. **Sorting Layer**: Default (ou mesmo layer do Player)
4. **Order in Layer**: 5 (para ficar na frente do corpo)

**Animator:**
1. Add Component → **Animator**
2. **Controller**: arraste `AC_Staff`
3. **Apply Root Motion**: ❌ desmarcado

**BoxCollider2D:**
1. Add Component → **Box Collider 2D**
2. **Is Trigger**: ✅ marcado
3. **Enabled**: ❌ DESMARCADO (começa desativado)
4. **Size**: X: 0.2, Y: 0.3

**StaffWeapon Script:**
1. Add Component → procure por **Staff Weapon**
2. No Inspector do script:
   - **Weapon Collider**: arraste o BoxCollider2D
   - **Weapon Sprite Renderer**: arraste o SpriteRenderer
   - **Weapon Animator**: arraste o Animator

#### 4.3 Salvar como Prefab

1. Arraste o GameObject `StaffWeapon` da Hierarchy para `Assets/Prefabs/Weapons/`
2. Deve criar o prefab `StaffWeapon.prefab`
3. Delete o GameObject da Hierarchy (não é mais necessário)

---

### **FASE 5: Criar WeaponData ScriptableObject**

#### 5.1 Criar Staff_Basic

1. No Project, vá para `Assets/Resources/Weapons/`
2. Clique direito → **Create → Weapons → Weapon Data**
3. Renomeie: `Staff_Basic`

#### 5.2 Configurar no Inspector

Com `Staff_Basic` selecionado:

**Weapon Info:**
- **Weapon Name**: "Wizard Staff"
- **Weapon Type**: Staff
- **Weapon Icon**: (opcional, arraste um sprite icon)

**Combat Stats:**
- **Damage**: 3
- **Attack Range**: 1.0
- **Attack Speed**: 1.0
- **Cooldown**: 0.5

**Visual & Animation:**
- **Animator Controller**: arraste `AC_Staff`
- **Attack Offset**: X: 0.15, Y: 0

**Item Integration:**
- **Item Reference**: (deixe vazio por enquanto)

---

### **FASE 6: Modificar Wizard Player Prefab**

#### 6.1 Abrir o Prefab

1. No Project, vá para `Assets/Characters/Wizard/`
2. Duplo clique em `Player.prefab` para abrir no Prefab Mode

#### 6.2 Adicionar WeaponSlot Child

1. Na Hierarchy do prefab, clique direito em **Player** → **Create Empty**
2. Renomeie: `WeaponSlot`
3. Transform → Position: (0, 0, 0)

#### 6.3 Adicionar WeaponManager Component

1. Com `WeaponSlot` selecionado, Add Component → **Weapon Manager**
2. No Inspector do WeaponManager:
   - **Weapon Slot**: arraste o próprio `WeaponSlot` (self reference)
   - **Available Weapons**:
     - Clique no **+** para adicionar elemento
     - Arraste `Staff_Basic` para o Element 0
   - **Staff Weapon Prefab**: arraste o prefab `StaffWeapon`
   - **Sword Weapon Prefab**: (deixe vazio por enquanto)

#### 6.4 Configurar WizardController

1. Selecione o GameObject **Player** (root do prefab)
2. No componente **Wizard Controller**:
3. Role até a seção **Weapon System**
4. **Weapon Manager**: arraste o child `WeaponSlot`

#### 6.5 Salvar e Sair

1. No topo da janela, clique em **Save**
2. Clique na setinha < ao lado de "Player" para sair do Prefab Mode

---

### **FASE 7: Testes no Unity**

#### 7.1 Abrir Cena de Teste

1. Abra a cena `Assets/Scenes/Magic.unity`
2. Entre em **Play Mode**

#### 7.2 Testar Equip do Staff

**Via Inspector (Runtime):**
1. Com o jogo rodando, selecione o Wizard na Hierarchy
2. No Inspector do **Wizard Controller**, role até os métodos públicos
3. Clique direito no componente → **Debug**
4. Encontre o método `EquipWeapon(WeaponData weaponData)`
5. Arraste `Staff_Basic` para o campo
6. Clique no botão de play/invoke

**Você deve ver:**
- ✅ Staff aparece como child de WeaponSlot
- ✅ Staff anima junto com corpo (idle/walk)
- ✅ Log no console: "[WeaponManager] Wizard Staff equipada com sucesso!"

#### 7.3 Testar Ataque

1. Com staff equipado, pressione o botão de ataque (Fire/Mouse)
2. Verificar:
   - ✅ Animação de ataque do corpo toca
   - ✅ Animação de ataque do staff toca sincronizada
   - ✅ Movimento trava durante ataque
   - ✅ Movimento destrava ao final

#### 7.4 Testar Direções

1. Ande em diferentes direções:
   - ⬆️ Cima (W)
   - ⬇️ Baixo (S)
   - ➡️ Direita (D)
   - ⬅️ Esquerda (A)
2. Verificar:
   - ✅ Staff muda animação idle/walk conforme direção
   - ✅ Ataque toca animação correta para direção

#### 7.5 Testar Colisão

1. Aproxime-se de um inimigo
2. Ataque o inimigo
3. Verificar no Console:
   - ✅ "[StaffWeapon] Wizard Staff causou 3 de dano em [Nome do Inimigo]"
   - ✅ Inimigo perde vida

#### 7.6 Testar Unequip

1. Pare o Play Mode
2. Entre em Play Mode novamente
3. Equipe o staff
4. No Inspector, chame método `UnequipWeapon()`
5. Verificar:
   - ✅ Staff desaparece
   - ✅ "[WeaponManager] Arma desequipada." no console
   - ✅ Pode re-equipar sem erros

---

## 🎯 Checklist Final

Antes de considerar a implementação completa, verifique:

### Scripts
- ✅ IWeapon.cs criado
- ✅ WeaponBase.cs criado
- ✅ WeaponData.cs criado
- ✅ WeaponManager.cs criado
- ✅ StaffWeapon.cs criado
- ✅ WizardController.cs modificado

### Animações
- ⬜ 9 animation clips do staff criados
- ⬜ AC_Staff controller configurado com blend trees
- ⬜ Parâmetros do animator configurados
- ⬜ Transições criadas corretamente

### Prefabs e Assets
- ⬜ StaffWeapon prefab criado e configurado
- ⬜ Staff_Basic WeaponData criado
- ⬜ Wizard Player.prefab atualizado com WeaponSlot

### Testes
- ⬜ Staff equipa corretamente
- ⬜ Animações sincronizam (corpo + staff)
- ⬜ Ataque funciona em todas as direções
- ⬜ Colisão com inimigos aplica dano
- ⬜ Unequip funciona sem erros

---

## 🐛 Troubleshooting

### Problema: "WeaponManager não encontrado"
**Solução:** Verifique se WeaponSlot é filho direto do Player no prefab.

### Problema: Staff não anima
**Solução:**
1. Verifique se AC_Staff está atribuído no StaffWeapon prefab
2. Verifique se parâmetros do animator estão corretos (moveX, moveY, isMoving, attack)

### Problema: Ataque não causa dano
**Solução:**
1. Verifique se inimigo tem tag "Enemy"
2. Verifique se inimigo tem componente HealthComponent
3. Verifique se BoxCollider2D do staff está configurado como Trigger

### Problema: Staff aparece atrás do corpo
**Solução:**
1. No StaffWeapon prefab, aumente Order in Layer do SpriteRenderer
2. Sugestão: Player = 0, Staff = 5

### Problema: Animações não sincronizam
**Solução:**
1. Verifique se WeaponManager.Update() está sendo executado
2. Adicione Debug.Log em SyncWeaponAnimatorWithParent()
3. Verifique se parentAnimator não é null

---

## 🚀 Próximos Passos (Futuro)

Depois de tudo funcionando:

1. **Criar outros tipos de armas:**
   - Wand (varinha mágica)
   - Spell Book (livro de magias)
   - Diferentes staffs (staff de fogo, gelo, etc.)

2. **Adicionar ataques mágicos:**
   - Projéteis (fireballs, ice shards)
   - Áreas de efeito (explosões, campos de força)
   - Buffs e debuffs

3. **Integração com inventário:**
   - Criar Item ScriptableObjects para armas
   - Equipar armas pegando itens no mundo
   - Menu de equipamento

4. **Polish:**
   - Efeitos visuais (particles, trails)
   - Sons de ataque
   - Tela de cooldown
   - Animações de equipar/desequipar

---

## 📞 Suporte

Se encontrar problemas durante a implementação:
1. Verifique os logs do console Unity
2. Verifique se todos os componentes estão atribuídos no Inspector
3. Revise o checklist acima
4. Entre em contato comigo para ajuda!

**Boa sorte com a implementação! 🧙‍♂️⚡**
