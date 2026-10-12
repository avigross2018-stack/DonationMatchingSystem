import mysql.connector

import const


def main():
    db = mysql.connector.connect(
        host=const.MYSQL_HOST,
        port=const.MYSQL_PORT,
        user=const.MYSQL_ROOT_USER,
        password=const.MYSQL_ROOT_PASSWORD,
    )

    cursor = db.cursor()

    try:
        cursor.execute(
            f"CREATE DATABASE IF NOT EXISTS {const.MYSQL_DATABASE} " "CHARACTER SET utf8mb4"
        )
        cursor.execute(f"USE {const.MYSQL_DATABASE}")

        queries = [
            """
            CREATE TABLE IF NOT EXISTS PopulationTypes (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(50) NOT NULL UNIQUE,
                Description VARCHAR(250),
                BaseEligibilityAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
                IsActive BOOLEAN NOT NULL DEFAULT TRUE,

                CONSTRAINT chk_population_base_eligibility
                    CHECK (BaseEligibilityAmount >= 0)
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS `Users` (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(50) NOT NULL,
                Phone VARCHAR(50) NOT NULL UNIQUE,
                PasswordHash VARCHAR(255) NOT NULL,
                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                IsActive BOOLEAN NOT NULL DEFAULT TRUE,
                UserType ENUM(
                    'DONOR',
                    'RECIPIENT',
                    'ADMIN'
                ) NOT NULL
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS Donors (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                UserId INT NOT NULL UNIQUE,

                FOREIGN KEY (UserId)
                    REFERENCES `Users`(Id)
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS Admins (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                UserId INT NOT NULL UNIQUE,

                FOREIGN KEY (UserId)
                    REFERENCES `Users`(Id)
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS Recipients (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                UserId INT NOT NULL UNIQUE,
                PopulationTypeId INT NOT NULL,
                RequestReason VARCHAR(250),
                IncomeAmount DECIMAL(10,2) NOT NULL,
                AmountOfChildren INT NOT NULL DEFAULT 0,
                EligibilityAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
                RemainingEligibility DECIMAL(10,2) NOT NULL DEFAULT 0,

                FOREIGN KEY (UserId)
                    REFERENCES `Users`(Id),

                FOREIGN KEY (PopulationTypeId)
                    REFERENCES PopulationTypes(Id),

                CONSTRAINT chk_recipient_income
                    CHECK (IncomeAmount >= 0),

                CONSTRAINT chk_recipient_children
                    CHECK (AmountOfChildren >= 0),

                CONSTRAINT chk_recipient_eligibility
                    CHECK (
                        EligibilityAmount >= 0
                        AND RemainingEligibility >= 0
                        AND RemainingEligibility <= EligibilityAmount
                    )
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS Donations (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                DonorId INT NOT NULL,
                PopulationTypeId INT NOT NULL,
                TotalAmount DECIMAL(10,2) NOT NULL,
                RemainingAmount DECIMAL(10,2) NOT NULL,
                MaxAmountPerRecipient DECIMAL(10,2) NOT NULL,
                Description VARCHAR(250),
                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                ExpirationDate DATETIME,
                Status ENUM(
                    'ACTIVE',
                    'COMPLETED',
                    'CANCELLED',
                    'EXPIRED'
                ) NOT NULL DEFAULT 'ACTIVE',

                FOREIGN KEY (DonorId)
                    REFERENCES Donors(Id),

                FOREIGN KEY (PopulationTypeId)
                    REFERENCES PopulationTypes(Id),

                CONSTRAINT chk_donation_amount
                    CHECK (
                        TotalAmount > 0
                        AND RemainingAmount >= 0
                        AND RemainingAmount <= TotalAmount
                        AND MaxAmountPerRecipient > 0
                    )
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS Redemptions (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                DonationId INT NOT NULL,
                RecipientId INT NOT NULL,
                HandledByAdminId INT NULL,
                Amount DECIMAL(10,2) NOT NULL,
                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                RedeemedAt DATETIME NULL DEFAULT NULL,
                Status ENUM(
                    'PENDING',
                    'COMPLETED',
                    'CANCELLED',
                    'REJECTED'
                ) NOT NULL DEFAULT 'PENDING',

                FOREIGN KEY (DonationId)
                    REFERENCES Donations(Id),

                FOREIGN KEY (RecipientId)
                    REFERENCES Recipients(Id),

                FOREIGN KEY (HandledByAdminId)
                    REFERENCES Admins(Id),

                CONSTRAINT chk_redemption_amount
                    CHECK (Amount >= 0)
            )
            """,
        ]

        for query in queries:
            cursor.execute(query)

        print("All tables created successfully!")

        cursor.execute("SHOW TABLES")

        for table in cursor.fetchall():
            print(table[0])

    except mysql.connector.Error as error:
        print(f"MySQL error: {error}")
        raise

    finally:
        cursor.close()
        db.close()


if __name__ == "__main__":
    main()
