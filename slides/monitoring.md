---
theme: default
title: "Monitoring"
info: "Surveiller un système distribué : polling, événements, logs et métriques"
author: ETML
transition: slide-left
mdc: true
---

# Monitoring

## D'un système informatique complexe

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 1 et 7
  </span>
</div>

---

# Plan de la présentation

1. **Importance** - SLI, SLO, SLA
2. **Deux approches** - actif et réactif
3. **Logging**
4. **Que mesurer ?**
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Importance

---

# Pourquoi surveiller

<v-clicks>

- **Disponibilité** : détecter vite les pannes
- **Performances** : latence, débit, saturation
- **Anticiper** : tendances, capacité, dégradation
- **Diagnostiquer** : corréler événements et métriques

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Sans monitoring fiable, on découvre les problèmes par les utilisateurs plutôt que par des signaux techniques.
</div>
</v-click>

---

# SLI, SLO, SLA

<v-clicks>

- **SLI** (*Indicator*) : valeur chiffrée qui mesure l'état d'un élément
- **SLO** (*Objective*) : la valeur que l'on vise
- **SLA** (*Agreement*) : la valeur minimale garantie aux utilisateurs

</v-clicks>

---
layout: section
---

# Partie 2
## Deux approches

---

# Actif ou réactif

|                      | **Actif (polling)**                        | **Réactif (événementiel)**                |
|----------------------|--------------------------------------------|-------------------------------------------|
| Initiative           | le monitoring interroge                    | le système surveillé signale              |
| Exemples             | ping, requête HTTP, health check           | logs, événements, alertes, **heartbeat**  |
| Avantages            | mesures régulières, indisponibilité franche| charge minimale                           |
| Limites              | charge, événements brefs manqués, échelle  | détection des crashs à gérer              |

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Interroger donne un état net mais charge le système. Écouter charge peu mais un <b>silence est ambigu</b> : mort ou rien à dire ?
</div>
</v-click>

---

# Détecter un crash en mode réactif

<v-clicks>

- **Heartbeat** : signal périodique dont l'**absence** est l'alerte
- **LWT** MQTT : le broker publie le testament
- Le broker publie ses métriques sur `$SYS/#` (clients connectés, messages reçus…)

</v-clicks>

---
layout: section
---

# Partie 3
## Logging

---

# Conserver au fil du temps

<v-clicks>

- Le monitoring donne une image **instantanée**, le log garde la mémoire
- Erreur **permanente** : intervention humaine nécessaire (alimentation qui « crame »)
- Erreur **transitoire** : disparaît seule (disque plein, puis libéré)
- Journaliser l'**apparition** et la **disparition**, pas chaque occurrence

</v-clicks>

---
layout: section
---

# Partie 4
## Que mesurer ?

---

# Symptôme et métrique

| Symptôme                 | Métrique qui le révèle                       |
|--------------------------|----------------------------------------------|
| messages en retard       | latence (p95), écart émission/réception      |
| effets doublés           | doublons détectés (même `id`)                |
| silence d'un service     | écart entre deux battements                  |
| ralentissement général   | messages par seconde, saturation             |
| service mort ou connecté | nombre de clients connectés                  |

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
Chaque mesure a un coût : la période d'un heartbeat se calcule (énergie).
</div>
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
<b>Exemple :</b> Le service meurt sans rien dire : un silence est ambigu. Heartbeat dont l'absence est l'alerte, ou LWT.
</div>
</v-click>

---

# Fallacy 7 : le transport est gratuit

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Chaque message coûte du temps, de l'énergie et de l'argent : les messages « pour rien » épuisent les batteries et la facture.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Mesurer, grouper les envois, espacer les heartbeats, calculer l'autonomie avant de choisir une fréquence.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Un polling chaque seconde sur 1000 nœuds sature le réseau. Passer à un heartbeat espacé ou à des événements.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Décider ce qu'on mesure, à quelle fréquence, et ce qu'on fait de l'alerte.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Surveiller coûte : mesurer utile, pas tout
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : puissance et énergie

</div>
</v-click>
