CREATE PROCEDURE PUT_QUESTION @QUESTION_ID INT, @RESERVED BIT, @QUESTION VARCHAR(4000), @TYPE INT, @CHECK_ALGORITHM INT
AS BEGIN
	
	UPDATE QUESTION 
	SET 
		RESERVED = @RESERVED,
		QUESTION = @QUESTION,
		[TYPE] = @TYPE,
		CHECK_ALGORITHM = @CHECK_ALGORITHM
	WHERE ID = @QUESTION_ID

	SELECT
		QUESTION.ID,
		QUESTION.TEST_ID,
		QUESTION.QUESTION_INDEX,
		QUESTION.RESERVED,
		QUESTION.QUESTION,
		QUESTION.[TYPE],
		QUESTION.CHECK_ALGORITHM
	FROM QUESTION
	WHERE QUESTION.ID = @QUESTION_ID;

END