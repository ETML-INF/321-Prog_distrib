---
theme: default
title: "Microservices"
info: "Une fonction, une équipe, un contrat : des services autonomes qui communiquent par messages"
author: ETML
transition: slide-left
mdc: true
---

# Microservices

## Une fonction, une équipe, un contrat

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 2, 5, 6 et 8
  </span>
</div>

---

# Plan de la présentation

1. **Définition** - services indépendants
2. **Communication** - UDP, TCP, HTTP, bus de messages
3. **Contrat** - le langage commun
4. **Bilan** - avantages et inconvénients
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: image
image: ./assets/microservice.jpeg
---

---
layout: section
---

# Partie 1
## Qu'est-ce qu'un microservice ?

---

# Des services indépendants

<v-clicks>

- Le système est décomposé en services **indépendants et autonomes**
- Chaque service a **une fonctionnalité précise**
- Développé, testé et déployé **séparément**
- Plus flexible, évolutif, maintenable… au prix d'un **réseau** entre les parties

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Les services ne partagent ni mémoire ni langage : ils communiquent par messages, et chacun peut tomber.
</div>
</v-click>

---
layout: section
---

# Partie 2
## Communiquer

---

# Les moyens de communication

| Moyen               | Garantie                               | Typique                                   |
|---------------------|----------------------------------------|-------------------------------------------|
| **UDP**             | pas de garantie, rapide                | jeux en ligne, vidéo en direct            |
| **TCP**             | fiable, dans l'ordre                   | transfert de fichiers, transactions       |
| **HTTP**            | requête/réponse sur TCP                | un service en appelle un autre et attend  |
| **Bus de messages** | asynchrone, publication/souscription   | RabbitMQ, Kafka, MQTT                     |

---

# Synchrone ou asynchrone

<div class="grid grid-cols-2 gap-6">
<div>

### HTTP, synchrone

```http
POST /api/orders/checkout
```

```json
{"success": true}
```

L'appelant **attend** : si le voisin est lent ou mort, il l'est aussi.

</div>
<v-click>
<div>

### Bus, asynchrone

```json
{"event":"user_created",
 "data":{"userId":12345,
         "email":"john.doe@example.com"}}
```

Le service abonné traite **quand il le peut**. Pas d'attente, messages conservés si panne.

</div>
</v-click>
</div>

---
layout: section
---

# Partie 3
## Un contrat par service

---

# Le contrat

<v-clicks>

- Chaque service a son équipe : ce qui les relie est un **contrat**
- Contrat = topics ou routes, forme des messages, erreurs
- Un contrat ambigu rend le service **inutilisable**
- Langage commun (sérialisation), pas code partagé : chaque équipe choisit son langage

</v-clicks>

---
layout: section
---

# Partie 4
## Bilan

---

# Avantages et inconvénients

| Thème         | Avantage                           | Inconvénient                                   |
|---------------|------------------------------------|------------------------------------------------|
| Charge        | **Scalabilité** par service        | **Complexité** : plus d'éléments à gérer       |
| Pannes        | **Résilience** : les autres continuent | **Latence** : chaque appel traverse le réseau |
| Évolution     | **Évolutivité** indépendante       | **Données** réparties, cohérence à assurer     |
| Technologies  | **Flexibilité** de langage         | **Sécurité** : chaque lien est une surface d'attaque |

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
La résilience n'est pas automatique : savoir qu'un voisin est mort (LWT), réessayer intelligemment, accepter les doublons (idempotence).
</div>
</v-click>

---
layout: section
---

# Fallacies associées
## Le piège et comment l'éviter

---

# Fallacy 2 : la latence est nulle

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Les temps de réponse varient et explosent : un appel synchrone bloque l'appelant, des timeouts mal réglés propagent la panne.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Timeouts explicites, communication asynchrone, regrouper les appels, pas d'appel réseau dans une boucle.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Le service commandes attend le service paiement en HTTP : si paiement est lent, commandes l'est aussi. Un événement asynchrone découple les deux.
</div>
</v-click>

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
<b>Exemple :</b> Remplacer <code>192.168.1.12:5000</code> par un topic <code>services/stock/#</code> : le service peut migrer sans toucher aux clients.
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
<b>Exemple :</b> L'équipe Stock passe <code>qty</code> en <code>quantity</code> un vendredi soir. Avec un contrat versionné <code>v1</code> et <code>v2</code> en parallèle, personne ne tombe.
</div>
</v-click>

---

# Fallacy 8 : le réseau est homogène

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Chaque équipe a son langage, ses versions, ses conventions de dates : un message valide pour l'une est illisible pour l'autre.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Format ouvert, contrat écrit (JSON Schema), dates en ISO 8601 UTC, versionner, ne jamais renommer un champ sans prévenir.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Stock en C#, Notifications en Python : ils ne partagent que le contrat JSON, jamais le code.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Un microservice = une fonction, une équipe, un contrat.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Concevoir chaque appel comme s'il pouvait échouer
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : l'idempotence

</div>
</v-click>
