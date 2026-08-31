# Versus Fighting — Contexte projet

## Présentation

Jeu de combat 2D évoluant dans un environnement 3D, inspiré de Super Smash Bros., développé sous Unity.

Objectif : une architecture propre, modulaire et facilement extensible permettant d'ajouter de nouveaux personnages, attaques et mécaniques sans modifier le code existant.

Chaque personnage est un Yokai possédant un élément (feu, eau, terre, foudre, etc.) qui influencera plus tard les interactions entre personnages.

Le projet privilégie une architecture orientée composants respectant autant que possible les principes SOLID.

## Architecture générale

### PlayerGameplay

Représente un joueur. Il possède : les différents contrôleurs, la machine à états (HFSM), les références vers les composants Unity, les données temporaires du joueur.

Il ne contient **pas** la logique spécifique d'un personnage.

### Character

Chaque prefab de personnage possède un composant `Character`, qui contient : les statistiques du personnage, les attaques, les hitbox, les données d'animation, tous les éléments propres au personnage.

Ainsi un même système (`PlayerGameplay`) peut contrôler plusieurs personnages différents simplement en changeant le prefab.

### State Machine (HFSM)

Le gameplay est organisé sous forme d'une Hierarchical Finite State Machine.

États principaux (exemples) : Idle, Move, Jump, Attack, Knocked, etc. Chaque état possède les méthodes classiques : Enter, Update, FixedUpdate, Exit.

La machine à états orchestre le gameplay mais délègue la logique métier aux différents contrôleurs.

### Contrôleurs

La logique est répartie dans des composants spécialisés, chacun responsable d'un seul domaine.

Contrôleurs portés par `PlayerGameplay` : `PlayerInputController`, `JumpController`, `AttackController`, `KnockbackController`, `DamageController`, `CharacterCollisionController`.

Contrôleurs portés par le prefab `Character` : `CharacterAnimatorController`, `VFXManager`, `HitboxManager`, `HurtBoxManager`, `CharacterAttackLibrary`.

*État actuel :* le déplacement horizontal n'a pas encore de contrôleur dédié — il est appliqué directement par `PlayerMoveState` et `PlayerDashState`. L'orientation visuelle non plus : elle est calculée par `PlayerGameplay.OrientationCheck()` et appliquée par `CharacterAnimatorController.VisualOrientationUpdate()`.

### Données (ScriptableObjects)

Le projet utilise énormément les ScriptableObjects, notamment pour : statistiques des personnages, statistiques des attaques, paramètres divers.

ScriptableObjects existants : `CharacterStatData` (stats de déplacement, saut, poids, durée de knocked) et `AttackDataSO` (dégâts, angle et puissance d'éjection).

Objectif : que toutes les données de gameplay soient configurables depuis l'éditeur.

### Système d'attaques

Les attaques sont identifiées par un enum `AttackTypes` (NeutralTilt, SideTilt, UpTilt, DownTilt, Nair, Fair, Bair, Dair).

Chaque attaque possède un `AttackDataSO` contenant les infos de gameplay (dégâts, knockback, angle, durée, etc.). Les attaques sont référencées dans le `Character` via `CharacterAttackLibrary` afin que chaque personnage puisse avoir ses propres statistiques.

### Hitbox / Hurtbox

Chaque personnage possède plusieurs Hitbox et une ou plusieurs Hurtbox.

Les Hitbox restent présentes sur le personnage mais sont activées/désactivées pendant les animations via des Animation Events.

Lorsqu'une Hurtbox détecte une Hitbox : elle construit un `HitData` (attaquant, `AttackDataSO`, point de contact) et le transmet au `KnockbackController` de la victime. Ce contrôleur encaisse le %, calcule le vecteur d'éjection, puis déclenche `PlayerKnockedState` qui l'applique au Rigidbody.

### Animations

Pilotées par `CharacterAnimatorController`. Les changements d'état déclenchent les animations.

Les Animation Events servent notamment à : activer une Hitbox, désactiver une Hitbox, notifier la fin d'une attaque.

### Input

Unity Input System. Le multijoueur local repose sur `PlayerInputManager` ("Join Players When Button Is Pressed"). Chaque `PlayerGameplay` possède son propre `PlayerInput`.

### Caméra

La caméra suit dynamiquement tous les joueurs. `MainCamera` maintient une liste de cibles (`trackingTargets`) et calcule le centre de leur boîte englobante sur l'axe X pour recentrer la caméra de manière fluide.

## Philosophie de développement

Le projet privilégie : une architecture modulaire, un faible couplage, des responsabilités bien séparées, une forte utilisation de la composition plutôt que de l'héritage, des données externalisées dans des ScriptableObjects, des systèmes facilement extensibles.

Lorsqu'une nouvelle mécanique est ajoutée, elle doit s'intégrer dans cette architecture sans introduire de dépendances inutiles.

## Objectifs à moyen terme

Plusieurs personnages jouables ; des éléments (feu, eau, terre, foudre…) influençant les combats ; davantage d'états dans la HFSM ; un système complet de knockback ; davantage d'attaques aériennes et au sol ; une architecture suffisamment générique pour accueillir de nouveaux personnages avec un minimum de code spécifique.

## Consignes pour un agent IA

Lorsqu'il intervient sur ce projet, l'agent doit :

- respecter l'architecture existante avant de proposer une refonte ;
- privilégier les composants spécialisés plutôt que centraliser la logique dans `PlayerGameplay` ;
- éviter les dépendances fortes entre systèmes ;
- proposer des solutions compatibles avec le Unity Input System, les ScriptableObjects et la HFSM ;
- privilégier les solutions extensibles à long terme plutôt que les correctifs rapides ;
- expliquer les choix d'architecture lorsqu'ils impactent l'organisation du projet ;
- conserver une séparation claire entre les données (ScriptableObjects), les contrôleurs (logique métier) et la machine à états (orchestration).

## Unity natif (OOB) vs code custom — règle prioritaire

Toujours privilégier les fonctionnalités **out-of-the-box de Unity** et les solutions **configurables depuis l'Inspecteur** plutôt que d'écrire une nouvelle classe C# from scratch, tant que le besoin peut être couvert par un composant natif (Layout Groups, Animator/Animation Events, ParticleSystem, Timeline, Physics, etc.) ou une configuration simple de ScriptableObject/prefab.

"Extensible" et "SOLID" ne veulent pas dire "toujours créer un système générique custom par défaut". Premier réflexe : est-ce que Unity fournit déjà l'outil adapté ? Un système custom ne se justifie que si aucune fonctionnalité native ne couvre correctement le besoin, ou si la logique est spécifique au jeu (HFSM, contrôleurs de gameplay, ScriptableObjects de stats).

**Conséquence pour l'agent :** face à une nouvelle fonctionnalité ou un problème d'architecture, présenter systématiquement les deux angles quand c'est pertinent, plutôt qu'une seule proposition :

1. **Option Unity natif / config Inspecteur** — composants standards, aucun ou peu de code, pas de nouvelle classe de logique.
2. **Option code custom** — nouvelle classe/système, avec les compromis (flexibilité, contrôle fin) vs le coût (plus de code à maintenir, plus de surface pour des bugs).

Laisser l'utilisateur trancher selon le besoin réel plutôt que de partir directement sur la solution la plus élaborée.

*Exemple de référence* : placement des HUD joueurs selon le nombre de joueurs (2 → gauche/droite, 3 → + un au milieu, etc.). Option custom envisagée : un `PlayerHUDManager` + un `ScriptableObject` de table d'ancres par nombre de joueurs. Option native retenue : un simple `HorizontalLayoutGroup` (`Child Force Expand Width = true`) comme container dans lequel chaque HUD est instancié en enfant — Unity recalcule lui-même le placement à chaque ajout/retrait, sans script supplémentaire.
