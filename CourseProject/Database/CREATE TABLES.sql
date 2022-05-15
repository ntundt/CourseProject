USE COURSE_PROJECT;

CREATE TABLE "USER" (
    ID INT CONSTRAINT USER_PK PRIMARY KEY NOT NULL IDENTITY(1, 1),
    NAME VARCHAR(64),
    DATE_CREATED DATETIME DEFAULT GETDATE() NOT NULL,
    LOGIN VARCHAR(64), -- make unique
    PASSWORD_SHA256 CHAR(64)
);

-- Make user.login unique if not null, found on the Internet
CREATE UNIQUE NONCLUSTERED INDEX UQ_Party_SamAccountName
ON dbo."USER"(LOGIN)
WHERE LOGIN IS NOT NULL;

CREATE TABLE ACCESS_TOKEN (
    TOKEN CHAR(64) CONSTRAINT ACCESS_TOKEN_PK PRIMARY KEY NOT NULL,
    USER_ID INT CONSTRAINT ACCESS_TOKEN_OWNER FOREIGN KEY REFERENCES "USER"(ID) NOT NULL
);

CREATE TABLE TEST (
    ID INT CONSTRAINT TEST_PK PRIMARY KEY NOT NULL IDENTITY(1, 1),
    AUTHOR_USER_ID INT CONSTRAINT TEST_AUTHOR_USER_ID FOREIGN KEY REFERENCES "USER"(ID) NOT NULL,
    DATE_CREATED DATETIME DEFAULT GETDATE() NOT NULL,
    RESULTS_PUBLIC BIT DEFAULT 0 NOT NULL,
    CAN_NOT_REVIEW_QUESTION BIT DEFAULT 0 NOT NULL,
    ATTEMPTS INT DEFAULT 0 NOT NULL,
    TIME_LIMIT TIME DEFAULT '00:00:00' NOT NULL,
    PUBLIC_UNTIL DATETIME DEFAULT 0 NOT NULL,
    PRIVATE_UNTIL DATETIME DEFAULT 0 NOT NULL
)

CREATE TABLE QUESTION (
    ID INT CONSTRAINT QUESTION_PK PRIMARY KEY NOT NULL IDENTITY(1, 1),
    TEST_ID INT CONSTRAINT QUESTION_TEST_ID FOREIGN KEY REFERENCES TEST(ID) NOT NULL,
    RESERVED BIT DEFAULT 1 NOT NULL,
    QUESTION_INDEX INT NOT NULL,
    QUESTION NVARCHAR(4000) NOT NULL,
    TYPE INT NOT NULL,
    CHECK_ALOGORHYTM INT
)

CREATE TABLE ANSWER_OPTION (
    ID INT CONSTRAINT ANSWER_OPTION_PK PRIMARY KEY NOT NULL IDENTITY(1, 1),
    QUESTION_ID INT CONSTRAINT ANSWER_OPTION_QUESTION_ID FOREIGN KEY REFERENCES QUESTION(ID) NOT NULL,
    ANSWER NVARCHAR(500) NOT NULL,
    CORRECT BIT NOT NULL
)

CREATE TABLE ONGOING_TEST (
    ID INT CONSTRAINT ONGOING_TEST_ID PRIMARY KEY NOT NULL IDENTITY(1, 1),
    TEST_ID INT CONSTRAINT ONGOING_TEST_TEST_ID FOREIGN KEY REFERENCES TEST(ID) NOT NULL,
    RESPONDENT_USER_ID INT CONSTRAINT ONGOING_TEST_RESPONDENT_USER_ID FOREIGN KEY REFERENCES "USER"(ID) NOT NULL,
    STARTED DATETIME DEFAULT GETDATE() NOT NULL,
    ENDED DATETIME
)

CREATE TABLE USER_ANSWER (
    ID INT CONSTRAINT USER_ANSWER_PK PRIMARY KEY NOT NULL IDENTITY(1, 1),
    ONGOING_TEST_ID INT CONSTRAINT USER_ANSWER_ONGOING_TEST_ID FOREIGN KEY REFERENCES ONGOING_TEST(ID) NOT NULL,
    ANSWER_ID INT CONSTRAINT USER_ANSWER_ANSWER_ID FOREIGN KEY REFERENCES ANSWER_OPTION(ID),
    QUESTION_ID INT CONSTRAINT USER_ANSWER_QUESTION_ID FOREIGN KEY REFERENCES QUESTION(ID),
    ANSWER NVARCHAR(500),
    CONSTRAINT USER_ANSWER_VALID CHECK (ANSWER_ID IS NOT NULL OR (QUESTION_ID IS NOT NULL AND ANSWER IS NOT NULL))
)