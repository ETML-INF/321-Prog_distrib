---
theme: default
title: "Transactions"
info: "ACID, 2PC et sagas : cohérence entre services indépendants"
author: ETML
transition: slide-left
mdc: true
---

# Transactions

## Cohérence à terme, compensations

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 1 et 6
  </span>
</div>

---

# Plan de la présentation

1. **ACID** - propriétés d'une transaction
2. **Distribué** - coordinateur et 2PC
3. **Sagas** - compensations
4. **Idempotence** - reprise après échec
5. **Choisir** - comparaison et recommandations
6. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## ACID

---

# Qu'est-ce qu'une transaction ?

Une unité logique de travail, selon les propriétés **ACID** :

<v-clicks>

1. **Atomicité** : tout réussit ou tout échoue
2. **Cohérence** : l'état reste valide avant et après
3. **Isolation** : invisible aux autres avant le *commit*
4. **Durabilité** : l'effet est permanent, même en cas de panne

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Entre deux services indépendants, pas d'atomicité « gratuite ». Le pire n'est pas l'échec, c'est le <b>résultat inconnu</b>.
</div>
</v-click>

---
layout: section
---

# Partie 2
## Transactions distribuées

---

# Commande : trois sous-opérations

<div class="grid grid-cols-2 gap-6">
<div>

<v-clicks>

1. Le client passe commande
2. Le stock est mis à jour
3. Le paiement est validé

Si une étape échoue : *rollback* de toutes les autres.

</v-clicks>

</div>
<div>

![](./assets/dt.jpg)

</div>
</div>

---

# Two-Phase Commit

<div class="grid grid-cols-2 gap-6">
<div>

<v-clicks>

- Un **coordinateur**
- Phase 1 : tous confirment qu'ils *peuvent* valider
- Phase 2 : le coordinateur ordonne la validation

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
Complexité, latence, faible résilience : si le coordinateur tombe, la transaction reste bloquée.
</div>
</v-click>

</div>
<div>

![](./assets/2PC.jpg)

</div>
</div>

---

# Exemple MQTT en trois étapes

```mermaid {scale: 0.55}
sequenceDiagram
    participant Client
    participant Broker
    participant Commande
    participant Stock
    participant Paiement
    Client ->> Broker: orders/create
    Broker ->> Commande: orders/create
    Commande ->> Broker: stock/reserve
    Broker ->> Stock: stock/reserve
    Stock ->> Broker: stock/reserve/confirmed
    Broker ->> Commande: stock/reserve/confirmed
    Commande ->> Broker: payment/charge
    Broker ->> Paiement: payment/charge
    Paiement ->> Broker: payment/confirmed
```

---
layout: section
---

# Partie 3
## Sagas

---

# Compenser plutôt que coordonner

<v-clicks>

- Une **saga** : transaction « longue », chaque étape a une **action inverse**
- Exemple : créer la commande, sortir du stock, débiter. Le débit échoue : remettre en stock, supprimer la commande
- Pas de coordinateur central : résilience et scalabilité
- Adaptées aux microservices et à MQTT

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Chaque étape doit définir sa compensation, et les <b>états intermédiaires sont visibles</b> avant la fin.
</div>
</v-click>

---
layout: section
---

# Partie 4
## Rôle de l'idempotence

---

# Reprise après échec

<v-clicks>

- Chaque étape doit pouvoir être **répétée sans effet indésirable**
- **Identifiants uniques** pour détecter les doublons
- **Stockage des états** des transactions déjà traitées

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
Cas vicieux : le service exécute puis plante <b>avant de répondre</b>. L'appelant ignore si ça a eu lieu.
</div>
</v-click>

<v-click>
<div class="mt-4 p-3 bg-green-700 rounded text-green-200">
Seule réponse sûre : renvoyer la <b>même requête avec le même identifiant</b>, le service retourne le résultat de la première exécution.
</div>
</v-click>

---
layout: section
---

# Partie 5
## Choisir

---

# Comparaison des modèles

| Modèle            | Principe                                   | Convient à                           |
|-------------------|--------------------------------------------|--------------------------------------|
| Centralisé (ACID) | une transaction globale                    | monolithes, peu distribués           |
| Distribué (2PC)   | coordination par protocole commun          | cohérence forte nécessaire           |
| Saga              | étapes indépendantes + compensations       | microservices, asynchrone            |

<v-click>

### Recommandations

1. Chaque étape **idempotente**, identifiée
2. **Sagas** par défaut en distribué
3. **Suivre les états** (`en cours`, `payée`, `annulée`)
4. Outils : *Outbox pattern*, middleware transactionnel

</v-click>

---
layout: section
---

# Fallacies associées
## Le piège et comment l'éviter

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
<b>Exemple :</b> Le service paie puis plante avant de répondre : l'appelant ignore le résultat. Renvoyer la même requête avec le même identifiant.
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
<b>Exemple :</b> Banque et coopérative sont administrées séparément : pas de transaction ACID commune. Saga avec compensations et états visibles.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Idempotence + compensations : cohérence et résilience malgré les défaillances.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Étapes idempotentes, compensations explicites
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : design for failure

</div>
</v-click>
