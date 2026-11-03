# Puissance et énergie

> **Fallacy :** 7 (le transport est gratuit).  
> **À retenir :** la **puissance** (watt) est instantanée, l'**énergie** (wattheure) est la puissance multipliée par la **durée**. Un appareil qui ne consomme presque rien peut vider une batterie s'il agit trop souvent : la fréquence compte autant que l'intensité.

Regardez ce cycliste en train de gravir le Mont Ventoux.

![](../supports/assets/ventoux.jpg)

Ça n'en a pas l'air, mais la pente est raide (environ 10 %) ! Pour monter à 15 km/h, ce cycliste doit fournir une puissance d'environ 330 watts. C'est bien, mais pas une performance incroyable s'il atteint cette vitesse sans la maintenir.

**À retenir :** la puissance se mesure en **watts** (`[W]`), de manière **instantanée**.

Par contre, s'il maintient cette allure pendant les 21 km de la montée, chapeau !

![](../supports/assets/pente_ventoux.svg)

Il sera très fatigué au sommet, parce qu'il aura dépensé beaucoup d'**énergie**. Combien ?

- À 15 km/h, il passe 21 / 15 = 1,4 heure sur son vélo (1 h 24 min).
- La puissance s'exprime souvent en kilowatts : 330 W = 0,33 `[kW]`.
- À puissance constante : 1,4 × 0,33 = **0,462 `[kWh]`**.

**À retenir :** l'énergie se mesure en **kilowattheures** (`[kWh]`), sur une certaine **durée**.

> **énergie [kWh] = puissance [kW] × durée [h]**

---

![douche.jpg](assets/douche.jpg)

De retour chez lui, notre cycliste a bien mérité une douche de 10 minutes. Si elle est chauffée à l'électricité, cela consommera environ 2,5 `[kWh]` 🔌 !

## Et dans un système distribué ?

- Un appareil sur **batterie** a une réserve d'énergie : capacité (en Ah) × tension (en V) = énergie (en Wh). Par exemple 2 Ah × 3,7 V = 7,4 Wh.
- Son autonomie = énergie de la batterie / puissance moyenne consommée. La puissance moyenne dépend de la **fréquence d'envoi** : chaque message sorti de la veille coûte de l'énergie.
- Un **keep-alive** court, un heartbeat rapproché, un sondage fréquent : autant de messages « pour rien » qui épuisent l'appareil ([MQTT](mqtt.md#keep-alive)).
- Du côté d'un nœud qui produit et consomme (une maison connectée, une usine) : le **bilan énergétique** est la somme `durée × puissance` des producteurs moins celle des consommateurs.

Calculez : [D10](../activites/drills/D10-monitoring-energie.md), partie 3.
