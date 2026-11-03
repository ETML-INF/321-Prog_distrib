# UMDD · On se fait confiance, mais quand même…

Jusqu'ici tout reposait sur la confiance : n'importe qui peut déposer une spécialité qui n'est pas la sienne, consulter ou vider n'importe quel compte, et le broker est ouvert à tous.

| | |
|---|---|
| Prérequis | [D09](../../drills/D09-securite.md) · [C7](../../chat/C7-confiance.md) |
| Fallacy | 4 (réseau sûr), 6 (plusieurs administrateurs) |
| Contrat | amendement `v: 2` de l'enveloppe (décidé en C7), éventuellement par service |
| Exigence | [Sécurité](../../../thematiques/09-securite.md#exigences-du-fil-rouge) |

## Mission

Mettre en place, sur **vos** services, des mécanismes cryptographiques pour que :

1. une spécialité déposée à la coopérative **vienne bien de la BP qui est seule à savoir la faire** ;
2. une demande de **solde** vienne bien du **détenteur** du compte ;
3. un ordre de **transfert** vienne bien du détenteur du compte ;
4. les opérations bancaires, qui passent par un broker ouvert, ne puissent être **lues que par les personnes autorisées**.

## DoD

- Un transfert dont le `from` est usurpé est refusé (je le prouve avec un message forgé à la main).
- Un transfert rejoué à l'identique est refusé.
- Un observateur sur `dousville/#` ne lit pas le contenu des opérations bancaires.
- La classe sait qui est qui : j'explique d'où je tiens la clé publique (ou le secret partagé) des autres.

## À décider en classe (une décision par groupe de service, puis harmonisation)

| Décision | Options |
|---|---|
| Symétrique ou asymétrique ? | secret partagé par paire / paire de clés par BP |
| Qui atteste les identités ? | l'annuaire, le prof, un accord de la classe |
| Comment faire tourner une clé compromise ? | à quel coût ? |

## Indices

- Réutilisez votre code de signature de [C7](../../chat/C7-confiance.md), il est déjà testé contre modification / usurpation / rejeu.
- Pour la confidentialité, discutez ce qui doit rester lisible (routage, `type`) et ce qui peut être chiffré (`payload`).
- Vous pouvez aussi sécuriser **le transport** (TLS, authentification du broker) : le labo n'en dispose pas, mais décrivez où vous le placeriez.
