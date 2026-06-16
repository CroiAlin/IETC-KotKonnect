-- =============================================================
-- KotKonnect - Jeu de données de démonstration (SEED)
-- À exécuter APRÈS 01_KotKonnect_schema.sql
-- Réexécutable : vide les tables puis les repeuple (idempotent)
-- Encodage : UTF-8 / utf8mb4
--
-- COMPTES DE TEST -> mot de passe identique pour tous : Test1234!
--   Étudiants    : alice.martin@etu.be · bruno.lefevre@etu.be
--                  chloe.dubois@etu.be · lucas.gerard@etu.be
--   Propriétaires: marie.dupont@proprio.be · jean.bernard@proprio.be
--                  sophie.lambert@proprio.be
-- =============================================================

USE kotkonnect;
SET NAMES utf8mb4;

-- --- Remise à zéro (ordre indifférent grâce à la désactivation des FK) ---
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE MESSAGES;
TRUNCATE TABLE CONVERSATIONS;
TRUNCATE TABLE PAIEMENTS;
TRUNCATE TABLE RELEVES_COMPTEURS;
TRUNCATE TABLE BAUX;
TRUNCATE TABLE DOCUMENTS_CANDIDATURE;
TRUNCATE TABLE CANDIDATURES;
TRUNCATE TABLE PHOTOS_BIEN;
TRUNCATE TABLE BIENS;
TRUNCATE TABLE PROFILS;
TRUNCATE TABLE REFRESH_TOKENS;
TRUNCATE TABLE UTILISATEURS;
SET FOREIGN_KEY_CHECKS = 1;

-- -------------------------------------------------------------
-- UTILISATEURS  (mot de passe en clair = Test1234! pour tous)
-- -------------------------------------------------------------
INSERT INTO UTILISATEURS (UtilisateurID, Email, MotDePasseHash, Role) VALUES
(1, 'alice.martin@etu.be',      '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'ETUDIANT'),
(2, 'bruno.lefevre@etu.be',     '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'ETUDIANT'),
(3, 'chloe.dubois@etu.be',      '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'ETUDIANT'),
(4, 'lucas.gerard@etu.be',      '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'ETUDIANT'),
(5, 'marie.dupont@proprio.be',  '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'PROPRIETAIRE'),
(6, 'jean.bernard@proprio.be',  '$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'PROPRIETAIRE'),
(7, 'sophie.lambert@proprio.be','$2a$11$GwNVQZAnDUEyYFGS0Xi/hevlhXG9X68P8f6eSm4GNvd7QIztzazAa', 'PROPRIETAIRE');

-- -------------------------------------------------------------
-- PROFILS  (1-1 avec UTILISATEURS ; Ecole/BudgetMax NULL pour les proprios)
-- -------------------------------------------------------------
INSERT INTO PROFILS (UtilisateurID, Nom, Prenom, Telephone, Ville, Ecole, BudgetMax) VALUES
(1, 'Martin',  'Alice',  '+32470112233', 'Mons',              'UMons',     550.00),
(2, 'Lefevre', 'Bruno',  '+32471223344', 'Liege',             'ULiege',    600.00),
(3, 'Dubois',  'Chloe',  '+32472334455', 'Bruxelles',         'ULB',       700.00),
(4, 'Gerard',  'Lucas',  '+32473445566', 'Louvain-la-Neuve',  'UCLouvain', 500.00),
(5, 'Dupont',  'Marie',  '+32475556677', 'Mons',              NULL,        NULL),
(6, 'Bernard', 'Jean',   '+32476667788', 'Liege',             NULL,        NULL),
(7, 'Lambert', 'Sophie', '+32477778899', 'Bruxelles',         NULL,        NULL);

-- -------------------------------------------------------------
-- BIENS  (5 PUBLIE, 1 BROUILLON, 1 LOUE)
-- -------------------------------------------------------------
INSERT INTO BIENS (BienID, ProprietaireID, Titre, Description, Adresse, Ville, CodePostal, Surface, NombrePieces, LoyerBase, Charges, Statut) VALUES
(1, 5, 'Studio lumineux centre-ville',   'Studio renove a deux pas du campus, ideal premiere annee.', 'Rue des Etudiants 12',   'Mons',             '7000', 28.50, 1, 450.00,  60.00, 'PUBLIE'),
(2, 5, 'Chambre en colocation',          'Chambre meublee dans une colocation conviviale de 4 personnes.', 'Avenue de la Gare 8', 'Mons',             '7000', 16.00, 1, 320.00,  50.00, 'PUBLIE'),
(3, 5, 'Appartement 2 chambres',         'Grand appartement, en cours de preparation.',               'Rue de Nimy 45',         'Mons',             '7000', 65.00, 3, 720.00, 100.00, 'BROUILLON'),
(4, 6, 'Kot etudiant proche ULiege',     'Kot fonctionnel proche des facultes et des commerces.',     'Rue Saint-Gilles 102',   'Liege',            '4000', 20.00, 1, 380.00,  55.00, 'PUBLIE'),
(5, 6, 'Studio renove',                  'Studio entierement renove, cuisine equipee.',               'Boulevard de la Sauveniere 3','Liege',        '4000', 30.00, 1, 480.00,  70.00, 'LOUE'),
(6, 7, 'Loft spacieux',                  'Loft lumineux au coeur de Bruxelles, transports a proximite.','Rue du Marche 21',      'Bruxelles',        '1000', 55.00, 2, 850.00, 120.00, 'PUBLIE'),
(7, 7, 'Chambre meublee',                'Chambre cosy dans residence etudiante calme.',              'Place des Sciences 7',   'Louvain-la-Neuve', '1348', 14.00, 1, 350.00,  45.00, 'PUBLIE');

-- -------------------------------------------------------------
-- PHOTOS_BIEN  (galerie par bien)
-- -------------------------------------------------------------
INSERT INTO PHOTOS_BIEN (BienID, UrlImage, Ordre) VALUES
(1, 'https://loremflickr.com/640/480/studio?lock=151',          0),
(1, 'https://loremflickr.com/640/480/livingroom?lock=171',      1),
(1, 'https://loremflickr.com/640/480/kitchen?lock=206',         2),
(2, 'https://loremflickr.com/640/480/room?lock=91',             0),
(2, 'https://loremflickr.com/640/480/interior?lock=122',        1),
(3, 'https://loremflickr.com/640/480/apartment?lock=2',         0),
(4, 'https://loremflickr.com/640/480/bedroom?lock=5',           0),
(4, 'https://loremflickr.com/640/480/apartment?lock=21',        1),
(4, 'https://loremflickr.com/640/480/interior?lock=102',        2),
(5, 'https://loremflickr.com/640/480/studio?lock=33',           0),
(5, 'https://loremflickr.com/640/480/bedroom?lock=61',          1),
(6, 'https://loremflickr.com/640/480/apartment?lock=3',         0),
(6, 'https://loremflickr.com/640/480/livingroom?lock=1821',     1),
(6, 'https://loremflickr.com/640/480/kitchen?lock=19111654',    2),
(7, 'https://loremflickr.com/640/480/room?lock=101',            0),
(7, 'https://loremflickr.com/640/480/bedroom?lock=71',          1);

-- -------------------------------------------------------------
-- CANDIDATURES  (UNIQUE(BienID, EtudiantID) respecte)
-- -------------------------------------------------------------
INSERT INTO CANDIDATURES (CandidatureID, BienID, EtudiantID, MessageEtudiant, Statut, DateCandidature) VALUES
(1, 1, 1, 'Bonjour, je suis tres interessee par ce studio, serait-il possible de le visiter ?', 'ENVOYE',  '2026-06-10 09:15:00'),
(2, 1, 2, 'Etudiant serieux, non fumeur, disponible immediatement.',                            'VU',      '2026-06-10 14:30:00'),
(3, 1, 3, 'Je cherche un logement pour septembre, ce studio me conviendrait parfaitement.',     'REFUSE',  '2026-06-11 08:00:00'),
(4, 4, 1, 'Bonjour, le kot est-il toujours disponible pour la rentree ?',                       'ACCEPTE', '2026-06-09 16:45:00'),
(5, 4, 4, 'Tres motive, je peux fournir une garantie parentale.',                               'ENVOYE',  '2026-06-12 11:20:00'),
(6, 6, 2, 'Le loft m''interesse beaucoup, est-il meuble ?',                                     'VU',      '2026-06-12 18:05:00'),
(7, 2, 3, 'Bonjour, la chambre en colocation est-elle encore libre ?',                          'ENVOYE',  '2026-06-13 10:10:00'),
(8, 7, 4, 'Interesse par la chambre meublee a Louvain-la-Neuve.',                               'ENVOYE',  '2026-06-13 13:40:00'),
(9, 5, 2, 'Candidature pour le studio renove, dossier complet disponible.',                     'ACCEPTE', '2026-05-20 09:00:00');

-- -------------------------------------------------------------
-- DOCUMENTS_CANDIDATURE  (justificatifs uploades)
-- -------------------------------------------------------------
INSERT INTO DOCUMENTS_CANDIDATURE (CandidatureID, TypeDocument, UrlFichier) VALUES
(1, 'CARTE_ETUDIANT',     'https://example.com/docs/alice_carte_etudiant.pdf'),
(1, 'GARANTIE_PARENTALE', 'https://example.com/docs/alice_garantie.pdf'),
(4, 'CARTE_ETUDIANT',     'https://example.com/docs/alice_carte_etudiant.pdf'),
(9, 'CONTRAT_GARANT',     'https://example.com/docs/bruno_contrat_garant.pdf');

-- -------------------------------------------------------------
-- BAUX  (le bien 5 est LOUE -> bail SIGNE avec Bruno)
-- -------------------------------------------------------------
INSERT INTO BAUX (BailID, BienID, LocataireID, DateDebut, DateFin, LoyerMensuelTotal, Statut, LienContratPDF) VALUES
(1, 5, 2, '2026-09-01', '2027-08-31', 550.00, 'SIGNE', 'https://example.com/contrats/bail_bien5_bruno.pdf');

-- -------------------------------------------------------------
-- RELEVES_COMPTEURS  (index du bail 1)
-- -------------------------------------------------------------
INSERT INTO RELEVES_COMPTEURS (BailID, TypeEnergie, ValeurIndex, DateReleve) VALUES
(1, 'ELECTRICITE', 12500.50, '2026-09-01 10:00:00'),
(1, 'EAU',           340.00, '2026-09-01 10:05:00'),
(1, 'GAZ',          8900.00, '2026-09-01 10:10:00');

-- -------------------------------------------------------------
-- PAIEMENTS  (echeancier du bail 1)
-- -------------------------------------------------------------
INSERT INTO PAIEMENTS (BailID, Montant, DateEcheance, DatePaiementEffectif, Statut, StripeID) VALUES
(1, 550.00, '2026-09-05', '2026-09-03 08:30:00', 'PAYE',       'pi_3PseedSEPT01'),
(1, 550.00, '2026-10-05', NULL,                  'EN_ATTENTE', NULL),
(1, 550.00, '2026-11-05', NULL,                  'EN_ATTENTE', NULL);

-- -------------------------------------------------------------
-- CONVERSATIONS  (UNIQUE(BienID, EtudiantID))
-- -------------------------------------------------------------
INSERT INTO CONVERSATIONS (ConversationID, BienID, EtudiantID, ProprietaireID, DateDernierMessage) VALUES
(1, 1, 1, 5, '2026-06-10 10:00:00'),
(2, 4, 1, 6, '2026-06-09 17:30:00');

-- -------------------------------------------------------------
-- MESSAGES  (expediteur = un des participants de la conversation)
-- -------------------------------------------------------------
INSERT INTO MESSAGES (ConversationID, ExpediteurID, Contenu, DateEnvoi, EstLu) VALUES
(1, 1, 'Bonjour, le studio est-il toujours disponible ?',        '2026-06-10 09:20:00', TRUE),
(1, 5, 'Bonjour Alice, oui il est disponible. Quand voulez-vous visiter ?', '2026-06-10 09:45:00', TRUE),
(1, 1, 'Parfait, seriez-vous libre jeudi apres-midi ?',          '2026-06-10 10:00:00', FALSE),
(2, 1, 'Bonjour, je confirme mon interet pour le kot.',          '2026-06-09 16:50:00', TRUE),
(2, 6, 'Tres bien, votre dossier a ete accepte. Bienvenue !',    '2026-06-09 17:30:00', FALSE);
