# UMDD · Une heure pour Dousville

À terme chaque BP publiera ses horaires d'ouverture : il faut que tout le monde soit **à la même heure**.

| | |
|---|---|
| Prérequis | [D02](../../drills/D02-temps.md) · [D03](../../drills/D03-qos-retain.md) · [C2](../../chat/C2-ordre.md) · [C3](../../chat/C3-presence.md) |
| Fallacy | « tout le monde connaît l'heure », 2 (latence), 7 (coût) |
| Contrat | [time](../../contrats/time.schema.json), topic `dousville/time` |
| Exigence | [Temps](../../../thematiques/02-temps.md#exigences-du-fil-rouge) · [MQTT](../../../thematiques/03-mqtt.md#exigences-du-fil-rouge) |

## Mission

Un groupe écrit le **TimeService** : il se synchronise sur un serveur NTP public puis diffuse l'heure de Dousville. Les autres BP l'utilisent.

1. Le TimeService interroge un serveur NTP (par exemple `ch.pool.ntp.org`) et en déduit sa propre erreur d'horloge.
2. Il publie à intervalles réguliers un message `time` sur `dousville/time`.
3. Chaque BP s'abonne, **compare** avec son horloge locale et affiche : heure locale, heure de Dousville, décalage.
4. Les BP affichent leur état « ouvert/fermé » d'après l'heure de Dousville (horaires à définir par BP).

## DoD

- Les BP de toute la classe affichent la même heure de Dousville, à moins d'une seconde.
- J'affiche mon décalage local ; avec `--clock-skew 30` de [C2](../../chat/C2-ordre.md), je le vois.
- Le TimeService garde une valeur correcte quand le serveur NTP est injoignable (mode dégradé documenté).

## À décider en groupe (et à écrire)

- Fréquence de diffusion : 1 s ? 10 s ? 1 min ? Qu'est-ce que chaque choix coûte à 30 BP ? (fallacy 7)
- Que fait une BP qui ne reçoit plus rien depuis 1 min : continue avec son horloge, se déclare « hors temps », s'arrête ?
- Qui a raison quand deux sources se contredisent ?

## Indices

- Un message `time` met du temps à arriver. Si vous ajoutez 2 s de latence via le [proxy](../../infra/README.md), de combien se trompe votre décalage ? Relisez les formules de [D02](../../drills/D02-temps.md) (4 horodatages) : comment les adapter à un aller-retour MQTT ?
- Mode difficile : écrire le paquet NTP vous-même en UDP (48 octets) au lieu d'une bibliothèque.
- Filet de sécurité : un TimeService de référence tourne côté prof.
