# Fil rouge « Powercher » (maisons connectées)

Scénario existant : chaque étudiant construit une maison équipée d'appareils (consommateurs, producteurs, stockage) qui vit dans une ville simulée. La ville est animée par des services centraux :

- **Mère Nature** : diffuse l'heure et la météo ;
- **PowerBank** : distribue du cash ;
- **PowerDeal** : achète et vend de l'énergie ;
- **PowerWatch** : surveille les maisons.

Dépôt : [ETML-INF/powercher](https://github.com/ETML-INF/powercher) (broker RabbitMQ, voir `csharp/powercher25/compose.yaml`).  
Variante de labo : `docker compose --profile rabbitmq up -d` dans [infra](../../infra/README.md) (MQTT sur le port `1885`, utilisateur `ict`, mot de passe `321`).

> Ici le **langage de la maison** est celui du dépôt (C#). Le contrat et les drills restent ceux du module ; les services centraux imposent leur [protocole Powercher](https://github.com/ETML-INF/powercher/tree/main/Backend/Protocol) (plutôt que l'enveloppe du [contrat de base](../../contrats/README.md)), ce qui est en soi un cas d'**hétérogénéité** (fallacy 8) : à documenter, la maison doit traduire.

## Couverture des exigences

Cas « adaptateur » : le scénario impose son protocole, la maison traduit vers et depuis le [contrat de base](../../contrats/README.md). Exigences génériques : [contrat de scénario](../README.md#contrat-de-scenario). Services possibles : [catalogue](../services.md).

| Thématique | Couvert par | Manque |
|---|---|---|
| Sérialisation | messages `TOWN_ENVIRONMENT`, `HOUSE_STATUS`, `CASH` | trois entrées hostiles à formaliser |
| Temps | Mère Nature | décalage d'horloge injecté à écrire |
| MQTT | Hello Powercher, LWT | flux retenu / non retenu à expliciter |
| Microservices | maison comme service, minute de silence | délai de détection à chiffrer |
| Idempotence | dédoublonnage sur l'enveloppe | borne de mémoire à écrire |
| Transactions | achat/vente avec PowerDeal | scénario de panne au milieu à écrire |
| Résilience | timeouts, backoff | panne provoquée à écrire |
| Cluster et P2P | achat entre maisons (`recipientId`) | élection / perte d'un membre à écrire |
| Sécurité | réseau de confiance (AES) | usurpation et rejeu à écrire |
| Supervision | PowerWatch | battement et délai à écrire |
| Énergie | bilan énergétique | perte de messages à écrire |

## Mise en route (reprise des anciennes thématiques)

- Forker le dépôt [Powercher](https://github.com/ETML-INF/powercher) (« pour contribuer »).
- **Hello Powercher :** recevoir les messages de Mère Nature et afficher l'heure diffusée.
- **Poser les bases de la maison :** dans le projet Frontend, copier le formulaire `HouseUI` en `HouseUI_xxx` (`xxx` = vos initiales), modifier `BootstrapUI` pour ouvrir votre maison, l'équiper de consommateurs, calculer le bilan énergétique.
- **Advanced :** à l'aide du dépôt de la bibliothèque [MQTTnet](https://github.com/dotnet/MQTTnet/tree/master), corriger l'écho des messages (une maison reçoit ses propres messages) et prendre en compte le LWT (message d'adieu d'une maison morte, minute de silence).
- **Bilan énergétique** (algorithme de l'ancienne thématique « Puissance vs Énergie ») : à chaque message de Mère Nature, `durée = newTime − oldTime`; `consommation = Σ durée × puissance` des consommateurs en fonction ; `production = Σ durée × puissance actuelle` des producteurs (ajustée à la météo) ; `bilan = production − consommation`.
- **Achat / vente entre maisons ou avec PowerDeal (P2P) :** utiliser le `recipientId` et définir une stratégie d'allocation des topics en lien avec lui.

## Par thématique

### Sérialisation {#serialisation}
**Sérialisation.** Les messages `TOWN_ENVIRONMENT`, `HOUSE_STATUS`, `CASH`. Sauvegardez l'état de votre maison avant de l'arrêter, rechargez-le. ([powercher_v1](../../powercher_v1/README.md), [catalogue](../../powercher_catalog/README.md))  
Drill : [D01](../../drills/D01-serialisation.md) · Chat : [C1](../../chat/C1-hello.md)

### Temps et protocole {#temps-protocole}
**Temps et protocole.** *Hello Powercher* : recevoir l'heure diffusée par Mère Nature et l'afficher. Calculer la durée écoulée entre deux messages. ([Temps](../../../thematiques/02-temps.md), [MQTT](../../../thematiques/03-mqtt.md), [NTP](../../ntp1/README.md))  
Drill : [D02](../../drills/D02-temps.md), [D03](../../drills/D03-qos-retain.md) · Chat : [C2](../../chat/C2-ordre.md), [C3](../../chat/C3-presence.md)

### Microservices {#microservices}
**Microservices.** Chaque maison est un service ; ouvrez-la dans le Frontend (`HouseUI_xxx`). Les messages émis par une maison sont aussi reçus par elle : corrigez. Gérer LWT : une maison « morte » envoie un message d'adieu avec une citation philosophique, chacune observe une minute de silence.  
Drill : [D04](../../drills/D04-mort-noeud.md) · Chat : [C4](../../chat/C4-mort.md)

### Idempotence et transactions {#idempotence-transactions}
**Idempotence et transaction.** Messages d'argent : dédupliquer sur l'enveloppe, flag « agent idempotent » ([idempotence](../../idempotence/README.md)). Achat/vente d'énergie avec [PowerDeal](../../powerdeal/README.md) : une transaction en plusieurs messages. [Transat](../../transat/README.md).  
Drill : [D05](../../drills/D05-idempotence.md), [D06](../../drills/D06-transaction.md) · Chat : [C5](../../chat/C5-doublons.md)

### Résilience et cluster {#resilience}
**Résilience.** Timeouts sur les messages de *deal*, [exponential backoff](../../../supports/design4failure.md#strategie-de-retry). Achat entre maisons (P2P) avec le `recipientId`.  
Drill : [D07](../../drills/D07-retry.md), [D08](../../drills/D08-cluster.md) · Chat : [C6](../../chat/C6-reconnexion.md)

### Sécurité {#securite}
**Sécurité.** [Réseau de confiance](../../aie_confiance/README.md) : chiffrer les communications (AES, clé de 32 octets), un espion ne voit que du chiffré. Branche « confiance », liberté totale.  
Drill : [D09](../../drills/D09-securite.md) · Chat : [C7](../../chat/C7-confiance.md)

### Supervision et énergie {#supervision-energie}
**Énergie et supervision.** Bilan énergétique de la maison : `Σ(durée × puissance)` des producteurs moins celle des consommateurs, recalculé à chaque message de Mère Nature ([énergie](../../../supports/Puissance%20et%20énergie.md)). PowerWatch comme exemple de supervision, UPS < 40 % / > 60 % comme alertes.  
Drill : [D10](../../drills/D10-monitoring-energie.md) · Chat : [C8](../../chat/C8-sante.md)

### Défi {#defi}
**Défi.** Dernier tour de ville : l'**Energy Manager** autonome (UPS, achat/vente) sous pannes provoquées. Le défi voté s'applique à Powercher comme à UMDD.
