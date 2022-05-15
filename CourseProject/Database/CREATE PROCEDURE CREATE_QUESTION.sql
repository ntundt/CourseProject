CREATE PROCEDURE CREATE_QUESTION @TEST_ID INT AS
BEGIN
	
	DECLARE @INDEX INT = (SELECT MAX(QUESTION_INDEX) FROM QUESTION WHERE QUESTION.TEST_ID = @TEST_ID);

	INSERT INTO QUESTION (TEST_ID, RESERVED, QUESTION_INDEX, QUESTION, [TYPE])
	VALUES (@TEST_ID, 0, @INDEX + 1, '', 0);

	SELECT 
		CAST(SCOPE_IDENTITY() AS INT) ID,
		@TEST_ID TEST_ID,
		0 RESERVED,
		@INDEX + 1 QUESTION_INDEX,
		'' QUESTION,
		0 [TYPE],
		NULL CHECK_ALGORITHM;

END