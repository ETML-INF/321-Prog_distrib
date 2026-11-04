---
theme: default
title: "Puissance et énergie"
info: "Watts, wattheures et autonomie d'un appareil sur batterie"
author: ETML
transition: slide-left
mdc: true
---

# Puissance et énergie

## La fréquence compte autant que l'intensité

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacy 7 : le transport est gratuit
  </span>
</div>

---

# Plan de la présentation

1. **Puissance** - le Mont Ventoux
2. **Énergie** - puissance × durée
3. **Système distribué** - batteries et messages
4. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Puissance

---

# Un cycliste au Ventoux

<div class="grid grid-cols-2 gap-6">
<div>

![](./assets/ventoux.jpg)

</div>
<div>

<v-clicks>

- Pente d'environ 10 %
- À 15 km/h : environ **330 watts**
- La puissance se mesure en **watts** `[W]`, de manière **instantanée**

</v-clicks>

</div>
</div>

---
layout: section
---

# Partie 2
## Énergie

---

# Tenir 21 km

<div class="grid grid-cols-2 gap-6">
<div>

<v-clicks>

- 21 km à 15 km/h = 1,4 h
- 330 W = 0,33 kW
- 1,4 × 0,33 = **0,462 kWh**

</v-clicks>

</div>
<div>

![](./assets/pente_ventoux.svg)

</div>
</div>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
<b>énergie [kWh] = puissance [kW] × durée [h]</b>
</div>
</v-click>

---

# Et la douche ?

<div class="grid grid-cols-2 gap-6">
<div>

![](./assets/douche.jpg)

</div>
<div>

<v-clicks>

- Douche de 10 minutes chauffée à l'électricité
- Environ **2,5 kWh**, soit plus de cinq fois la montée du Ventoux

</v-clicks>

</div>
</div>

---
layout: section
---

# Partie 3
## Dans un système distribué

---

# Batterie et messages

<v-clicks>

- Énergie de la batterie : capacité (Ah) × tension (V) = Wh. Exemple : 2 Ah × 3,7 V = **7,4 Wh**
- Autonomie = énergie de la batterie / puissance moyenne
- La puissance moyenne dépend de la **fréquence d'envoi** : chaque message sorti de la veille coûte
- Keep-alive court, heartbeat rapproché, sondage fréquent : des messages « pour rien » qui épuisent l'appareil
- Nœud producteur et consommateur : **bilan énergétique** = Σ durée × puissance (producteurs) moins (consommateurs)

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
Un appareil qui consomme presque rien peut vider une batterie s'il agit trop souvent.
</div>
</v-click>

---
layout: section
---

# Fallacies associées
## Le piège et comment l'éviter

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
<b>Exemple :</b> Un capteur sur batterie de 7,4 Wh qui envoie chaque seconde se vide en quelques jours : espacer et grouper multiplie l'autonomie.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
La puissance est instantanée, l'énergie intègre la durée.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Espacer, grouper, mesurer
  </span>
</div>

# Questions ?
</v-click>
