---
theme: default
title: "Cluster, load balancing, failover"
info: "Plusieurs nœuds qui se présentent comme un seul service"
author: ETML
transition: slide-left
mdc: true
---

# Cluster

## Load balancing, failover, scalability

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 1, 5 et 6
  </span>
</div>

---

# Plan de la présentation

1. **Cluster** - un service, plusieurs nœuds
2. **Load balancing** - répartir la charge
3. **Failover** - détecter la panne
4. **Scalability** - ajouter des nœuds
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Le cluster

---

# Un ensemble coordonné

<v-clicks>

- Des nœuds qui coopèrent pour fournir un **service commun**
- Exemple : plusieurs centrales d'énergie, les maisons ne voient qu'**un service**
- Transparent pour l'utilisateur : répartition de charge, redirection d'un travail non terminé, ajout d'un nœud

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Le plus simple : un cluster <b>sans état</b>, n'importe quel nœud peut répondre.
</div>
</v-click>

---
layout: section
---

# Partie 2
## Load balancing

---

# Centralisé ou distribué

<div class="grid grid-cols-2 gap-6">
<div>

### Centralisé

Un *Master* assigne aux *Workers* :

<v-clicks>

- **Round Robin** : chacun son tour
- **Segmentation** : selon le client
- **Least Connections**
- **Least Load**
- **Random**

</v-clicks>

</div>
<v-click>
<div>

### Distribué

Les tâches sont présentées à tous, chacun « pioche » selon un calcul sur les attributs (**hash-based**).

<div class="mt-4 p-3 bg-green-700 rounded text-green-200">
MQTT 5 : les abonnements partagés donnent un round robin sans master.
</div>

</div>
</v-click>
</div>

---
layout: section
---

# Partie 3
## Failover

---

# Détecter la panne

<v-clicks>

- Centralisé : le Master ajuste sa liste de Workers
- Distribué : chaque nœud restant se reconfigure
- La difficulté : **comment savoir qu'un nœud est mort ?**
- **Heartbeat** : l'absence du signal régulier signifie la mort
- **Timeout** : question sans réponse dans le délai
- **Détection distribuée** : chaque nœud évalue les autres et vote

</v-clicks>

---

# Vote : la navette spatiale

![](./assets/sdC4j.jpg)

Les ordinateurs de bord de la navette évaluent l'état les uns des autres et votent.

---

# Attention : split brain

<v-clicks>

- Un nœud seulement **coupé** (pas mort) se croit toujours responsable
- Un autre prend sa place : **deux nœuds responsables**
- Service sans état (annuaire) : sans gravité
- Service avec état (banque) : **double effet**

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
D'où l'importance de l'idempotence.
</div>
</v-click>

---
layout: section
---

# Partie 4
## Scalability

---

# L'inverse de la panne

<v-clicks>

- On crée le nouveau nœud, sans le problème de détection
- Centralisé : ajuster la liste de Workers du Master
- Distribué : configurer la règle de répartition sur le nouveau nœud et reconfigurer les autres avec la même règle

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
<b>Exemple :</b> Un nœud ajouté au cluster doit être découvert et ajouté à la liste de Workers sans redéployer les clients.
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
<b>Exemple :</b> Deux équipes gèrent chacune un nœud avec une configuration différente : le round robin devient imprévisible. Une règle de répartition commune et versionnée l'évite.
</div>
</v-click>

---

# Fallacy 1 : le réseau est fiable

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Les messages se perdent, se dupliquent, les nœuds meurent sans prévenir.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Accusés de réception, retries avec backoff, idempotence, LWT et heartbeat pour détecter les morts.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Le nœud A est coupé, B prend le relais, A se croit toujours responsable (split brain) : deux débits. Idempotence et identifiants les rendent inoffensifs.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Load balancing, failover et scalability se matérialisent dans un cluster.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Sans état si possible, idempotent sinon
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : P2P

</div>
</v-click>
