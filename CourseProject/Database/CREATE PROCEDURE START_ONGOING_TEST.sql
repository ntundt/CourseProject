CREATE PROCEDURE START_ONGOING_TEST @TEST_ID INT, @RESPONDENT_USER_ID INT AS BEGIN
    INSERT INTO ONGOING_TEST (
        TEST_ID,
        RESPONDENT_USER_ID
    ) VALUES (
        @TEST_ID,
		@RESPONDENT_USER_ID
    )

    UPDATE ONGOING_TEST
    SET ONGOING_TEST.ENDED = ONGOING_TEST.STARTED + CAST((SELECT TEST.TIME_LIMIT FROM TEST WHERE TEST.ID = SCOPE_IDENTITY()) AS DATETIME)

    SELECT
        [USER].ID UID,
        [USER].NAME UNAME,
        [USER].DATE_CREATED UDATE_CREATED,
        [USER].LOGIN ULOGIN,
        [USER].PASSWORD_SHA256 UPASSWORD_SHA256,

        TEST.ID TID,
        TEST.AUTHOR_USER_ID TAUTHOR_USER_ID,
        TEST.DATE_CREATED TDATE_CREATED,
        TEST.RESULTS_PUBLIC TRESULTS_PUBLIC,
        TEST.CAN_NOT_REVIEW_QUESTION TCAN_NOT_REVIEW_QUESTION,
        TEST.ATTEMPTS TATTEMPTS,
        TEST.TIME_LIMIT TTIME_LIMIT,
        TEST.PUBLIC_UNTIL TPUBLIC_UNTIL,
        TEST.RESULTS_PUBLIC TRESULTS_PUBLIC,

        ONGOING_TEST.ID OID,
        ONGOING_TEST.TEST_ID OTEST_ID,
        ONGOING_TEST.RESPONDENT_USER_ID ORESPONDENT_USER_ID,
        ONGOING_TEST.[STARTED] OSTARTED,
        ONGOING_TEST.ENDED OENDED
    FROM
        ONGOING_TEST
        JOIN TEST ON TEST.ID = ONGOING_TEST.TEST_ID
        JOIN [USER] ON [USER].ID = ONGOING_TEST.RESPONDENT_USER_ID
    WHERE
        ONGOING_TEST.ID = SCOPE_IDENTITY();
END