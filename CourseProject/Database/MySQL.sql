CREATE TABLE `user` (
  `id` int PRIMARY KEY,
  `name` nvarchar(64),
  `date_created` timestamp,
  `login` varchar(64),
  `password_sha256` char(64)
);

CREATE TABLE `test` (
  `id` int PRIMARY KEY,
  `author_id` int,
  `date_created` timestamp,
  `public_until` timestamp,
  `private_until` timestamp
);

CREATE TABLE `question` (
  `id` int PRIMARY KEY,
  `author_id` int,
  `question` nvarchar(1024),
  `type` ENUM ('single_choise', 'multiple_choise', 'string'),
  `check` ENUM ('exact_match', 'partial_match', 'proportional_match')
);

CREATE TABLE `access_token` (
  `token` char(64) PRIMARY KEY,
  `user_id` int
);

CREATE TABLE `answer` (
  `id` int PRIMARY KEY,
  `question_id` int,
  `answer` nvarchar(64),
  `correct` boolean
);

CREATE TABLE `user_answer` (
  `ongoing_test_id` int PRIMARY KEY,
  `answer_id` int
);

CREATE TABLE `ongoing_test` (
  `id` int PRIMARY KEY,
  `user_id` int,
  `test_id` int,
  `start` timestamp
);

ALTER TABLE `test` ADD FOREIGN KEY (`author_id`) REFERENCES `user` (`id`);

ALTER TABLE `question` ADD FOREIGN KEY (`author_id`) REFERENCES `user` (`id`);

ALTER TABLE `access_token` ADD FOREIGN KEY (`user_id`) REFERENCES `user` (`id`);

ALTER TABLE `answer` ADD FOREIGN KEY (`question_id`) REFERENCES `question` (`id`);

ALTER TABLE `user_answer` ADD FOREIGN KEY (`ongoing_test_id`) REFERENCES `ongoing_test` (`id`);

ALTER TABLE `user_answer` ADD FOREIGN KEY (`answer_id`) REFERENCES `answer` (`id`);

ALTER TABLE `ongoing_test` ADD FOREIGN KEY (`user_id`) REFERENCES `user` (`id`);

ALTER TABLE `ongoing_test` ADD FOREIGN KEY (`test_id`) REFERENCES `test` (`id`);
