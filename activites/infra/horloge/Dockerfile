# Image des outils de temps (profil « temps », D02) :
# client/serveur NTP (chrony) et fausse horloge (faketime).
FROM debian:12-slim
RUN apt-get update \
 && apt-get install -y --no-install-recommends chrony faketime ca-certificates \
 && rm -rf /var/lib/apt/lists/*
# Configuration du serveur NTP local du labo (service « ntp »)
COPY lab.conf /etc/chrony/lab.conf
