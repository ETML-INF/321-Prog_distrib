---
theme: default
title: "Architecture P2P"
info: "Chaque nœud est à la fois client et serveur"
author: ETML
transition: slide-left
mdc: true
---

# Architecture P2P

## Sans serveur central

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 5 et 6
  </span>
</div>

---

# Plan de la présentation

1. **Rappels** - client-serveur, microservices
2. **P2P** - chaque nœud est un pair
3. **Avec un broker ?** - s'en approcher
4. **Choisir**
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: image
image: ./assets/p2p.gif
---

---
layout: section
---

# Partie 1
## Rappels

---

# Deux architectures connues

<v-clicks>

- **Client-serveur** : un client demande à un serveur centralisé qui répond (navigateur et serveur web)
- **Microservices** : services indépendants et autonomes sur des nœuds distants

</v-clicks>

---
layout: section
---

# Partie 2
## Peer-to-Peer

---

# Client et serveur à la fois

<v-clicks>

- Chaque **peer** agit comme client **et** serveur
- Déployés sur des machines distinctes, reliées par un réseau commun
- Tous ont les **mêmes fonctionnalités**
- Exemple : partage de fichiers avec BitTorrent

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Mêmes avantages et inconvénients que les microservices, et l'<b>évolutivité fonctionnelle</b> est limitée : tous les nœuds implémentent tout.
</div>
</v-click>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
On supprime le serveur central (et son point de défaillance), mais trouver ses pairs devient le problème.
</div>
</v-click>

---
layout: section
---

# Partie 3
## Et avec un broker ?

---

# Un P2P approché

<v-clicks>

- Un broker MQTT est un point central : pas de P2P pur
- Les pairs s'**annoncent** (présence), se **découvrent** (annuaire ou retain)
- Ils s'adressent par des topics dédiés : `chat/dm/<nom>`
- Le broker n'est qu'un tuyau, mais reste un point de défaillance (voir cluster)

</v-clicks>

---
layout: section
---

# Fallacies associées
## Le piège et comment l'éviter

---

# Fallacy 5 : la topologie ne change pas

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Des nœuds arrivent, partent, changent d'adresse : une adresse écrite en dur devient fausse.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Découverte (annuaire, présence MQTT), noms logiques, configuration externe, pas d'adresse IP dans le code.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Les pairs entrent et sortent sans cesse : annonce de présence (retain) et découverte par annuaire.
</div>
</v-click>

---

# Fallacy 6 : il n'y a qu'un administrateur

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Plusieurs équipes déploient à leur rythme avec leurs règles : une modification locale casse les autres.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Contrats explicites et versionnés, compatibilité ascendante, journaux signés, décisions de confiance documentées.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Chaque pair est administré par quelqu'un d'autre : aucun n'est cru sur parole, on valide et on signe.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Le choix dépend des besoins : scalabilité, coût, sécurité. Les deux se complètent souvent.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Pas de serveur central, mais il faut trouver ses pairs
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : la sécurité

</div>
</v-click>
