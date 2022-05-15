CREATE PROCEDURE QUESTION_SET_INDEX @QUESTION_ID INT, @NEW_INDEX INT AS
BEGIN
	DECLARE @OLD_INDEX INT, @TEST_ID INT;
	SELECT 
		@OLD_INDEX = QUESTION_INDEX, 
		@TEST_ID = TEST_ID 
	FROM QUESTION 
	WHERE ID = @QUESTION_ID; 
	
	UPDATE QUESTION 
	SET QUESTION.QUESTION_INDEX = @NEW_INDEX 
	WHERE QUESTION.ID = @QUESTION_ID;
	
	IF @NEW_INDEX > @OLD_INDEX BEGIN 
		UPDATE QUESTION 
		SET 
			QUESTION_INDEX = QUESTION_INDEX - 1 
		WHERE 
			TEST_ID = @TEST_ID 
			AND QUESTION_INDEX BETWEEN @OLD_INDEX AND @NEW_INDEX;
	END
	ELSE 
	BEGIN
		UPDATE QUESTION 
		SET 
			QUESTION_INDEX = QUESTION_INDEX + 1 
		WHERE 
			TEST_ID = @TEST_ID 
			AND QUESTION_INDEX BETWEEN @OLD_INDEX AND @NEW_INDEX;
	END
END