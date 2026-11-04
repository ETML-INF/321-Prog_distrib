---
theme: default
title: "Introduction au module 321"
info: "Programmer des systèmes distribués : démarche, thématiques et fallacies"
author: ETML
transition: slide-left
mdc: true
---

# Programmer des systèmes distribués

## Module ICT 321

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Introduction
  </span>
</div>

---
layout: image
image: ./assets/iot.jpg
---

---

# Plan de la présentation

1. **Le module** - compétence visée
2. **Pourquoi distribuer ?** - charge et fonctionnalités
3. **Les 8 fallacies** - fil rouge du module
4. **Les thématiques** - théorie, drill, chat, fil rouge
5. **Fonctionnement** - IA permise, à condition de comprendre

---
layout: section
---

# Partie 1
## Le module

---

# La compétence visée

<div class="mt-4 p-3 bg-blue-800 rounded text-blue-100">
<b>Programmer des systèmes distribués</b>
</div>

<v-clicks>

- Répartir des **fonctionnalités** et/ou de la **charge de travail** sur plusieurs ordinateurs
- Techniques, pratiques et technologies
- Exemples : maison intelligente, santé, agriculture, énergie

</v-clicks>

---

# Pourquoi distribuer ?

<v-clicks>

- **Charge** : répartir le travail (cluster, load balancing)
- **Pannes** : un nœud tombe, les autres continuent (failover)
- **Évolution** : ajouter des ressources progressivement (scalability)
- **Équipes** : une fonction, une équipe, un contrat (microservices)

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Au prix d'un <b>réseau</b> entre les parties. Et le réseau n'est pas ce que l'on croit.
</div>
</v-click>

---
layout: section
---

# Partie 2
## Les 8 fallacies

---

# Fil rouge du module

| #  | On croit que…              | #  | On croit que…                   |
|----|----------------------------|----|---------------------------------|
| 1  | Le réseau est fiable       | 5  | La topologie ne change pas      |
| 2  | La latence est nulle       | 6  | Il n'y a qu'un administrateur   |
| 3  | La bande passante est infinie | 7 | Le transport est gratuit       |
| 4  | Le réseau est sûr          | 8  | Le réseau est homogène          |

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Plus un neuvième : <b>chacun connaît l'heure avec précision</b>. Lire chaque énoncé en demandant : « de quelle croyance dépend-il ? »
</div>
</v-click>

---
layout: section
---

# Partie 3
## Les thématiques

---

# Onze thématiques

<div class="grid grid-cols-2 gap-6">
<div>

1. **Sérialisation** - fallacies 3, 8
2. **Temps** - NTP, heure, 2
3. **MQTT** - 1, 2
4. **Microservices** - 1, 5, 6
5. **Idempotence** - 1
6. **Transactions** - 1, 6

</div>
<div>

7. **Résilience** - 1, 2
8. **Cluster et P2P** - 5, 6
9. **Sécurité** - 4, 6
10. **Supervision** - 1, 7
11. **Énergie** - 7

</div>
</div>

---

# Ce que contient chaque thématique

<v-clicks>

1. **Théorie** : les supports et ces slides
2. **Drill** : un mécanisme éprouvé à la main, résultat attendu à chaque étape (≈ 20 min, sans IA)
3. **Chat** : une amélioration guidée de **son** chat, dans le langage de son choix
4. **Fil rouge** : le même mécanisme dans un décor (UMDD, Powercher, Pedaloland), moins guidé

</v-clicks>

---

# Une architecture IoT typique

<v-clicks>

- **Synchronisation** : NTP ou horloges logiques pour un ordre cohérent
- **Échanges** : données sérialisées (JSON, Protobuf), validées contre un contrat
- **Bus** : MQTT pour découpler les services
- **Résilience** : idempotence, retries, cluster, supervision

</v-clicks>

---
layout: section
---

# Partie 4
## Fonctionnement

---

# Règles du jeu

<v-clicks>

- L'**ordre** et le découpage en séances suivent le planning
- Un **labo Docker** : broker et proxy de pannes sur un seul hôte
- Un **contrat de base** commun, quel que soit le langage
- L'IA est permise à condition de **comprendre**

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-green-700 rounded text-green-200">
Avant de chercher le bug, chercher la fallacy.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Un système distribué, c'est un système où des pannes arrivent, et où il faut quand même tenir sa promesse.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Concevoir en sachant que le réseau ment
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : la sérialisation

</div>
</v-click>
