---
theme: default
title: "Les 8 erreurs des systèmes distribués"
info: "Les croyances par défaut sur un réseau, et qui sont fausses"
author: ETML
transition: slide-left
mdc: true
---

# Les 8 erreurs des systèmes distribués

## Les fallacies

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Sun Microsystems : Deutsch, Gosling et d'autres
  </span>
</div>

---

# Pourquoi ces fallacies ?

<v-clicks>

- Ce sont les croyances que l'on a **par défaut** sur un réseau
- Elles sont **fausses**
- Chaque erreur finit en panne en production

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Pour débugger : avant de chercher le bug, chercher la fallacy.
</div>
</v-click>

---

# 1 à 4

| #  | On croit que…              | En réalité…                              | Parade                                      |
|----|----------------------------|------------------------------------------|---------------------------------------------|
| 1  | Le réseau est fiable       | messages perdus, dupliqués, nœuds morts  | accusés, retry, idempotence, LWT            |
| 2  | La latence est nulle       | elle varie, elle explose                 | timeout, asynchrone                         |
| 3  | La bande passante est infinie | limitée et partagée                   | formats compacts, ne pas tout envoyer       |
| 4  | Le réseau est sûr          | quelqu'un écoute, modifie, rejoue        | chiffrement, signature                      |

---

# 5 à 8

| #  | On croit que…              | En réalité…                              | Parade                                      |
|----|----------------------------|------------------------------------------|---------------------------------------------|
| 5  | La topologie ne change pas | des nœuds arrivent, partent, bougent     | découverte (annuaire), pas d'adresses en dur|
| 6  | Un seul administrateur     | plusieurs équipes, plusieurs règles      | contrats explicites, versionnés             |
| 7  | Le transport est gratuit   | il coûte temps, énergie, argent          | mesurer, grouper, espacer                   |
| 8  | Le réseau est homogène     | langages, versions, systèmes différents  | formats ouverts, contrat                    |

---

# Un neuvième : l'heure

<v-clicks>

- « À tout endroit du réseau, chacun connaît l'heure avec précision »
- Faux aussi : NTP rapproche les horloges, Lamport ordonne malgré elles

</v-clicks>

---

# Comment s'en servir

<v-clicks>

- **Lire un énoncé** : de quelle croyance cette fonctionnalité dépend-elle ?
- **Écrire un prompt** : nommer la fallacy menaçante est un critère d'évaluation
- **Débugger** : chercher la fallacy avant le bug

</v-clicks>

---
layout: section
---

# Fallacies associées
## Un bug, trois fallacies

---

# Exemple : « le paiement a été débité deux fois »

<v-clicks>

- **Fallacy 1** : le réseau a perdu la réponse, le client a renvoyé. Mieux faire : idempotence, identifiant unique par intention
- **Fallacy 2** : le timeout était trop court pour un serveur chargé. Mieux faire : timeout adapté, backoff avec jitter
- **Fallacy 6** : l'équipe Paiement a changé le comportement sans prévenir. Mieux faire : contrat versionné

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-green-700 rounded text-green-200">
Avant de chercher le bug dans le code, demander : de quelle croyance dépend ce comportement ?
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Chaque croyance par défaut est une panne en attente.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Nommer la fallacy avant de coder
  </span>
</div>

# Questions ?
</v-click>
