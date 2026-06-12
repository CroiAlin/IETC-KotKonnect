-- =============================================================
-- KotKonnect - Script de création de la base de données
-- MySQL 8+ / InnoDB / utf8mb4
-- Réexécutable : DROP + CREATE à chaque lancement
-- =============================================================

DROP DATABASE IF EXISTS kotkonnect;
CREATE DATABASE kotkonnect CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE kotkonnect;

-- -------------------------------------------------------------
-- UTILISATEURS : compte de base (étudiant ou propriétaire)
-- -------------------------------------------------------------

CREATE TABLE UTILISATEURS (
    UtilisateurID   INT AUTO_INCREMENT PRIMARY KEY,
    Email           VARCHAR(255) NOT NULL,
    MotDePasseHash  VARCHAR(255) NOT NULL,
    Role            VARCHAR(20)  NOT NULL,
    DateCreation    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT UQ_Utilisateurs_Email UNIQUE (Email),
    CONSTRAINT CHK_Utilisateurs_Role CHECK (Role IN ('ETUDIANT', 'PROPRIETAIRE'))
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- REFRESH_TOKENS : jetons de renouvellement JWT
-- CASCADE : un token sans utilisateur n'a aucun sens
-- -------------------------------------------------------------

CREATE TABLE REFRESH_TOKENS (
    TokenID         INT AUTO_INCREMENT PRIMARY KEY,
    UtilisateurID   INT          NOT NULL,
    Token           VARCHAR(255) NOT NULL,
    DateExpiration  DATETIME     NOT NULL,
    Revoque         BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_RefreshTokens_Utilisateur FOREIGN KEY (UtilisateurID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE,
    CONSTRAINT UQ_RefreshTokens_Token UNIQUE (Token)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- PROFILS : informations personnelles (relation 1-1 avec UTILISATEURS)
-- UNIQUE(UtilisateurID) garantit le 1-1
-- -------------------------------------------------------------

CREATE TABLE PROFILS (
    ProfilID        INT AUTO_INCREMENT PRIMARY KEY,
    UtilisateurID   INT          NOT NULL,
    Nom             VARCHAR(100) NOT NULL,
    Prenom          VARCHAR(100) NOT NULL,  
    Telephone       VARCHAR(20)  NULL,
    Ville           VARCHAR(100) NULL,
    Ecole           VARCHAR(150) NULL,
    BudgetMax       DECIMAL(8,2) NULL,

    CONSTRAINT FK_Profils_Utilisateur FOREIGN KEY (UtilisateurID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE,
    CONSTRAINT UQ_Profils_Utilisateur UNIQUE (UtilisateurID),
    CONSTRAINT CHK_Profils_BudgetMax CHECK (BudgetMax IS NULL OR BudgetMax >= 0)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- BIENS : kots mis en location
-- Soft delete via Statut = 'SUPPRIME' (jamais de DELETE physique)
-- RESTRICT : on ne supprime pas un utilisateur qui possède des biens
-- -------------------------------------------------------------

CREATE TABLE BIENS (
    BienID          INT AUTO_INCREMENT PRIMARY KEY,
    ProprietaireID  INT           NOT NULL,
    Titre           VARCHAR(150)  NOT NULL,
    Description     TEXT          NULL,
    Adresse         VARCHAR(255)  NOT NULL,
    Ville           VARCHAR(100)  NOT NULL,
    CodePostal      VARCHAR(10)   NOT NULL,
    Surface         DECIMAL(6,2)  NOT NULL,
    NombrePieces    INT           NOT NULL,
    LoyerBase       DECIMAL(8,2)  NOT NULL,
    Charges         DECIMAL(8,2)  NOT NULL DEFAULT 0,
    Statut          VARCHAR(20)   NOT NULL DEFAULT 'BROUILLON',
    CONSTRAINT FK_Biens_Proprietaire FOREIGN KEY (ProprietaireID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE RESTRICT,
    CONSTRAINT CHK_Biens_Statut CHECK (Statut IN ('BROUILLON', 'PUBLIE', 'LOUE', 'SUPPRIME')),
    CONSTRAINT CHK_Biens_Surface CHECK (Surface > 0),
    CONSTRAINT CHK_Biens_NombrePieces CHECK (NombrePieces > 0),
    CONSTRAINT CHK_Biens_LoyerBase CHECK (LoyerBase >= 0),
    CONSTRAINT CHK_Biens_Charges CHECK (Charges >= 0)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- PHOTOS_BIEN : galerie d'un bien
-- CASCADE : donnée purement technique, suit le bien
-- -------------------------------------------------------------

CREATE TABLE PHOTOS_BIEN (
    PhotoID         INT AUTO_INCREMENT PRIMARY KEY,
    BienID          INT          NOT NULL,
    UrlImage        VARCHAR(500) NOT NULL,
    Ordre           INT          NOT NULL DEFAULT 0,
    CONSTRAINT FK_PhotosBien_Bien FOREIGN KEY (BienID)
        REFERENCES BIENS(BienID) ON DELETE CASCADE
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- CANDIDATURES : un étudiant postule à un bien
-- UNIQUE(BienID, EtudiantID) : une seule candidature par étudiant et par bien
-- -------------------------------------------------------------

CREATE TABLE CANDIDATURES (
    CandidatureID   INT AUTO_INCREMENT PRIMARY KEY,
    BienID          INT          NOT NULL,
    EtudiantID      INT          NOT NULL,
    MessageEtudiant TEXT         NULL,
    Statut          VARCHAR(20)  NOT NULL DEFAULT 'ENVOYE',
    DateCandidature DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT FK_Candidatures_Bien FOREIGN KEY (BienID)
        REFERENCES BIENS(BienID) ON DELETE CASCADE,
    CONSTRAINT FK_Candidatures_Etudiant FOREIGN KEY (EtudiantID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE,
    CONSTRAINT UQ_Candidatures_Bien_Etudiant UNIQUE (BienID, EtudiantID),
    CONSTRAINT CHK_Candidatures_Statut CHECK (Statut IN ('ENVOYE', 'VU', 'ACCEPTE', 'REFUSE'))
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- DOCUMENTS_CANDIDATURE : justificatifs uploadés
-- -------------------------------------------------------------

CREATE TABLE DOCUMENTS_CANDIDATURE (
    DocumentID      INT AUTO_INCREMENT PRIMARY KEY,
    CandidatureID   INT          NOT NULL,
    TypeDocument    VARCHAR(50)  NOT NULL,
    UrlFichier      VARCHAR(500) NOT NULL,
    DateUpload      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT FK_Documents_Candidature FOREIGN KEY (CandidatureID)
        REFERENCES CANDIDATURES(CandidatureID) ON DELETE CASCADE
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- BAUX : contrat de location = donnée légale
-- RESTRICT : interdit de supprimer un bien ou un utilisateur lié à un bail
-- -------------------------------------------------------------

CREATE TABLE BAUX (
    BailID            INT AUTO_INCREMENT PRIMARY KEY,
    BienID            INT          NOT NULL,
    LocataireID       INT          NOT NULL,
    DateDebut         DATE         NOT NULL,
    DateFin           DATE         NOT NULL,
    LoyerMensuelTotal DECIMAL(8,2) NOT NULL,
    Statut            VARCHAR(20)  NOT NULL DEFAULT 'EN_ATTENTE',
    LienContratPDF    VARCHAR(500) NULL,

    CONSTRAINT FK_Baux_Bien FOREIGN KEY (BienID)
        REFERENCES BIENS(BienID) ON DELETE RESTRICT,
    CONSTRAINT FK_Baux_Locataire FOREIGN KEY (LocataireID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE RESTRICT,
    CONSTRAINT CHK_Baux_Statut CHECK (Statut IN ('EN_ATTENTE', 'SIGNE', 'RESILIE')),
    CONSTRAINT CHK_Baux_Dates CHECK (DateFin > DateDebut),
    CONSTRAINT CHK_Baux_Loyer CHECK (LoyerMensuelTotal >= 0)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- RELEVES_COMPTEURS : index énergie liés à un bail
-- -------------------------------------------------------------

CREATE TABLE RELEVES_COMPTEURS (
    ReleveID        INT AUTO_INCREMENT PRIMARY KEY,
    BailID          INT           NOT NULL,
    TypeEnergie     VARCHAR(20)   NOT NULL,
    ValeurIndex     DECIMAL(10,2) NOT NULL,
    DateReleve      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT FK_Releves_Bail FOREIGN KEY (BailID)
        REFERENCES BAUX(BailID) ON DELETE CASCADE,
    CONSTRAINT CHK_Releves_TypeEnergie CHECK (TypeEnergie IN ('EAU', 'GAZ', 'ELECTRICITE')),
    CONSTRAINT CHK_Releves_ValeurIndex CHECK (ValeurIndex >= 0)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- PAIEMENTS : échéancier d'un bail = donnée financière
-- RESTRICT : un bail avec paiements ne peut pas être supprimé
-- -------------------------------------------------------------

CREATE TABLE PAIEMENTS (
    PaiementID           INT AUTO_INCREMENT PRIMARY KEY,
    BailID               INT          NOT NULL,
    Montant              DECIMAL(8,2) NOT NULL,
    DateEcheance         DATE         NOT NULL,
    DatePaiementEffectif DATETIME     NULL,
    Statut               VARCHAR(20)  NOT NULL DEFAULT 'EN_ATTENTE',
    StripeID             VARCHAR(255) NULL,

    CONSTRAINT FK_Paiements_Bail FOREIGN KEY (BailID)
        REFERENCES BAUX(BailID) ON DELETE RESTRICT,
    CONSTRAINT CHK_Paiements_Statut CHECK (Statut IN ('EN_ATTENTE', 'PAYE', 'EN_RETARD')),
    CONSTRAINT CHK_Paiements_Montant CHECK (Montant > 0)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- CONVERSATIONS : fil de discussion bien <-> étudiant <-> propriétaire
-- UNIQUE(BienID, EtudiantID) : une seule conversation par étudiant et par bien
-- -------------------------------------------------------------

CREATE TABLE CONVERSATIONS (
    ConversationID     INT      NOT NULL AUTO_INCREMENT PRIMARY KEY,
    BienID             INT      NOT NULL,
    EtudiantID         INT      NOT NULL,
    ProprietaireID     INT      NOT NULL,
    DateDernierMessage DATETIME NULL,

    CONSTRAINT FK_Conversations_Bien FOREIGN KEY (BienID)
        REFERENCES BIENS(BienID) ON DELETE CASCADE,
    CONSTRAINT FK_Conversations_Etudiant FOREIGN KEY (EtudiantID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE,
    CONSTRAINT FK_Conversations_Proprietaire FOREIGN KEY (ProprietaireID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE,
    CONSTRAINT UQ_Conversations_Bien_Etudiant UNIQUE (BienID, EtudiantID)
) ENGINE=InnoDB;

-- -------------------------------------------------------------
-- MESSAGES : messages d'une conversation
-- -------------------------------------------------------------

CREATE TABLE MESSAGES (
    MessageID       INT AUTO_INCREMENT PRIMARY KEY,
    ConversationID  INT      NOT NULL,
    ExpediteurID    INT      NOT NULL,
    Contenu         TEXT     NOT NULL,
    DateEnvoi       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    EstLu           BOOLEAN  NOT NULL DEFAULT FALSE,
    
    CONSTRAINT FK_Messages_Conversation FOREIGN KEY (ConversationID)
        REFERENCES CONVERSATIONS(ConversationID) ON DELETE CASCADE,
    CONSTRAINT FK_Messages_Expediteur FOREIGN KEY (ExpediteurID)
        REFERENCES UTILISATEURS(UtilisateurID) ON DELETE CASCADE
) ENGINE=InnoDB;