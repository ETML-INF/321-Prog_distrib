# Le chat (spirale individuelle)

Chaque étudiant construit **son** chat, dans **le langage de son choix**, en 8 étapes guidées. À chaque thématique du module, le chat reçoit **une** amélioration qui met en pratique le thème du jour. C'est un banc d'essai de plomberie, sans lien avec un scénario : il utilise sa propre racine de topics `chat/`. Ses briques sont ensuite réutilisées dans le [fil rouge](../fil-rouge/README.md).

```mermaid
flowchart LR
  C1[C1 Hello] --> C2[C2 Ordre] --> C3[C3 Présence] --> C4[C4 Mort] --> C5[C5 Doublons] --> C6[C6 Reconnexion] --> C7[C7 Confiance] --> C8[C8 Santé]
```

| Étape | Thématique | Le chat apprend à… | Drill préalable | Fallacy | Théorie |
|---|---|---|---|---|---|
| [C1](C1-hello.md) | [Sérialisation](../../thematiques/01-serialisation.md) | envoyer et recevoir des messages valides | [D01](../drills/D01-serialisation.md) | 3, 8 | [Sérialisation](../../supports/Sérialisation.md) |
| [C2](C2-ordre.md) | [Temps](../../thematiques/02-temps.md) | afficher dans le bon ordre | [D02](../drills/D02-temps.md) | heure | [NTP](../../supports/ntp.md) |
| [C3](C3-presence.md) | [MQTT](../../thematiques/03-mqtt.md) | savoir qui est là, parler en privé | [D03](../drills/D03-qos-retain.md) | 1, 2 | [MQTT](../../supports/mqtt.md) |
| [C4](C4-mort.md) | [Microservices](../../thematiques/04-microservices.md) | annoncer sa propre mort | [D04](../drills/D04-mort-noeud.md) | 1, 5 | [MQTT](../../supports/mqtt.md#last-will-and-testament-lwt) |
| [C5](C5-doublons.md) | [Idempotence](../../thematiques/05-idempotence.md) | ignorer les doublons | [D05](../drills/D05-idempotence.md) | 1 | [Idempotence](../../supports/idempotence.md) |
| [C6](C6-reconnexion.md) | [Résilience](../../thematiques/07-resilience.md) | survivre à la coupure du broker | [D07](../drills/D07-retry.md) | 1, 2 | [Design for failure](../../supports/design4failure.md) |
| [C7](C7-confiance.md) | [Sécurité](../../thematiques/09-securite.md) | prouver l'auteur, refuser le rejeu | [D09](../drills/D09-securite.md) | 4, 6 | [Sécurité](../../supports/securite.md) |
| [C8](C8-sante.md) | [Supervision](../../thematiques/10-supervision.md) | dire comment il va, sobrement | [D10](../drills/D10-monitoring-energie.md) | 7 | [Puissance et énergie](../../supports/Puissance%20et%20énergie.md) |

## Règles du jeu

- **Individuel**, un dossier par étudiant (`chat-<prénom>`), un `git commit` par étape terminée.
- **Langage libre.** Contrainte : une bibliothèque MQTT mature (client MQTT 3.1.1 au minimum). Vous justifiez votre choix en C1.
- **IA autorisée si vous comprenez.** Chaque étape se termine par une rubrique *Terminé quand* : on vous demandera de l'expliquer, voir [évaluation](../../evaluation/grille.md).
- **Le contrat fait foi** : [activites/contrats](../contrats/README.md). Vous n'inventez ni topics ni champs sans le faire amender.
- **Les outils de vérification sont dans le labo** : [infra](../infra/README.md). `DC` = `docker compose exec broker` (à lancer depuis `activites/infra`).
- **Un `NOTES.md` à la racine de votre dossier** : 2 lignes par étape (« ce que j'ai compris », « ce qui a cassé »). Il servira de base à l'oral.

## Les canaux

Les topics du chat sont sous la racine `chat/` : `chat/all`, `chat/dm/<prénom>`, `chat/presence/<prénom>`, `chat/log/<prénom>`. Ils sont **communs à la classe** sur le broker partagé. En cas de conflit (deux élèves avec le même prénom, un canal détourné), c'est un sujet de discussion normal, et les canaux pourront être ajustés par l'enseignant ou la classe.

## Étape finale : réutiliser son chat

Pointez votre chat sur la racine de votre [scénario](../fil-rouge/README.md) **par simple configuration, sans modifier le code**, et observez votre présence et votre battement à côté de ceux des autres nœuds. Si vous devez changer du code, la racine était codée en dur : c'est la leçon (fallacy 8, le réseau n'est pas homogène). La plomberie (connexion, enveloppe, validation, reconnexion, signature, battement) est celle que vous utiliserez pour les services de votre groupe.
