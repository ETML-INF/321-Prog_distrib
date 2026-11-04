# MQTT

Un protocole de messagerie léger, ses garanties de livraison (QoS), ses présences (retain) et ses testaments (LWT).

**À l'issue de cette thématique, vous serez capable de :**
- prédire qui reçoit quoi selon le QoS, la session et le retain
- choisir un protocole (MQTT / REST / RPC) pour un besoin donné et le justifier
- utiliser les wildcards et séparer les flux par topics
- expliquer ce que le broker garantit et ce qu'il ne garantit pas

**Fallacies :**

- [1 — Le réseau est fiable](../supports/fallacies.md#fallacy-1)
- [2 — La latence est nulle](../supports/fallacies.md#fallacy-2)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [MQTT](../supports/mqtt.md)
- [Kahoot A](https://create.kahoot.it/share/mqtt/8545db98-717d-4ecb-9e2f-1349eb200161) · [Kahoot B](https://create.kahoot.it/details/c8177a44-0898-44e5-9c5b-4d72099a28be)
- Slides : [MQTT](../slides/mqtt/){target="_self"}

## Drill
- [D03 — MQTT : QoS, session, retain](../activites/drills/D03-qos-retain.md)

## Chat
- [C3 — Présence et messages privés](../activites/chat/C3-presence.md)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Source de temps](../activites/fil-rouge/services.md#cat-source-de-temps)
- **Doit introduire :** un premier service branché sur le bus, avec au moins un flux d'état retenu et un flux d'événements non retenu
- **Preuve attendue :** un nœud qui arrive tard voit tout de suite l'état courant, et ne reçoit pas les anciens événements
- **Décision à documenter :** le QoS retenu pour chaque flux, avec la raison

## Fil rouge

| Fil rouge  | Activité                                                                        |
|------------|---------------------------------------------------------------------------------|
| UMDD       | [TimeService (premier service sur le bus)](../activites/fil-rouge/umdd/time.md) |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#temps-protocole)         |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                              |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |

Activité alternative : [Footchat](../activites/footchat/README.md) (échauffement MQTT)
