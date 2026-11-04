---
theme: default
title: "Design for failure"
info: "Détecter, réagir, reprendre : concevoir en sachant que les pannes arriveront"
author: ETML
transition: slide-left
mdc: true
---

# Design for failure

## Les pannes sont des données du problème

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 1, 2 et 5
  </span>
</div>

---

# Plan de la présentation

1. **Pourquoi** - la loi de Murphy
2. **Réaction** - timeout, fail fast, mode dégradé, redondance
3. **Reprise** - retry, backoff, jitter, circuit breaker
4. **Synthèse**
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Embrasser la loi de Murphy

---

# Pourquoi ça tombe en panne

<div class="grid grid-cols-2 gap-6">
<div>

<v-clicks>

- Plus de nœuds, plus de risques de panne
- Le réseau accroît encore le risque
- La diversité matérielle et logicielle ajoute des surprises
- Déraisonnable de croire que tout ira bien

</v-clicks>

</div>
<div>

![](./assets/resilience2.png)

</div>
</div>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
<b>Résilience</b> : capacité à se récupérer rapidement et efficacement après une perturbation. Trois phases : détection, réaction, reprise.
</div>
</v-click>

---
layout: section
---

# Partie 2
## Réaction

---

# Qualifier l'erreur

<v-clicks>

- **Transitoire** : peut disparaître seule (pic de charge, réseau brièvement coupé)
- **Permanente** : exige une intervention (bug, mauvaise configuration, mauvaise URL)

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Un <b>timeout</b> est le délai maximal accordé à une opération distante. Il ne déclenche pas automatiquement un retry : il déclenche une <b>décision</b> (réessayer, abandonner, dégrader).
</div>
</v-click>

---

# Fail fast

<v-clicks>

- Détecter et traiter l'erreur le plus **en amont** possible
- Exemple : contrôler l'année de naissance dès la soumission, pas lors du calcul d'âge
- Aller plus loin : l'interface **empêche** de soumettre une valeur inacceptable

</v-clicks>

---

# Mode dégradé et throttling

<v-clicks>

- **Mode dégradé** : ne fournir qu'une partie des services (consulter, mais plus créer)
- **Throttling** : réduire volontairement la performance pour alléger la charge
- **Token Bucket** : un seau de jetons par client, une requête consomme un jeton, seau vide = rejet

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-800 rounded text-blue-100">
Diminuer le nombre de jetons distribués réduit la capacité apparente.
</div>
</v-click>

---

# Arrêt propre, redondance, cluster

<v-clicks>

- **Graceful shutdown** : ne pas interrompre brutalement, transférer le service à un autre
- **Redondance** : jumeau **passif** prêt à prendre le relais (*failover*)
- **Cluster** : plusieurs composants **actifs** vus comme un seul service

</v-clicks>

---
layout: section
---

# Partie 3
## Reprise

---

# L'effet domino

<v-clicks>

1. Un serveur très sollicité répond plus lentement
2. Des clients atteignent le timeout et **renvoient** leur requête
3. Le serveur traite le travail en double, répond encore plus lentement
4. De plus en plus de clients réessayent… jusqu'au crash

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
Un retry naïf peut faire pire que la panne.
</div>
</v-click>

---

# Backoff exponentiel et jitter

<v-clicks>

- Espacer les tentatives : 1 s, 2 s, 4 s, 8 s… avec un **plafond** (*backoff cap*)
- Avantage : on laisse le système récupérer
- Problème : tous les clients frappent **en même temps** à chaque vague (billetterie de Paléo)
- **Jitter** : ajouter du **hasard** au délai

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-green-700 rounded text-green-200">
Un retry n'est sûr que si l'opération est <b>idempotente</b>.
</div>
</v-click>

---

# Circuit breaker

Un fusible qui protège les services critiques en interrompant les appels vers un service défaillant.

<v-clicks>

1. **Closed** : tout va bien, les requêtes passent
2. **Open** : les requêtes **échouent immédiatement**, sans solliciter le service cible
3. **Half-open** : après un délai, quelques requêtes test ; succès = *closed*, échec = *open*

</v-clicks>

---
layout: section
---

# Partie 4
## Synthèse

---

# Un bon système distribué

<v-clicks>

- Conçu avec la conviction que des pannes surviendront (**design for failure**)
- Réagit aussi tôt que possible (**fail fast**)
- Compte un maximum de nœuds capables de reprendre seuls (**résilience**)

</v-clicks>

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
<b>Exemple :</b> Un retry sans idempotence débite deux fois. Retry avec identifiant unique et backoff avec jitter : sûr.
</div>
</v-click>

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
<b>Exemple :</b> Un timeout de 30 s bloque un thread pendant une panne. Un timeout de 2 s plus un circuit breaker libère les ressources.
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
<b>Exemple :</b> Un service redémarré change d'adresse : les clients qui réessaient l'ancienne adresse échouent pour toujours. Redécouverte via l'annuaire.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Anticiper les échecs, les accepter, prévoir comment les traiter.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Détecter, réagir, reprendre
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : cluster et P2P

</div>
</v-click>
