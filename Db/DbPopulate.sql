USE trimaniadb
DELIMITER //
DROP FUNCTION IF EXISTS `RandString`;
CREATE FUNCTION `RandString`(length SMALLINT(3)) RETURNS varchar(100) CHARSET utf8
begin
    SET @returnStr = '';
    SET @allowedChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz@!#$%&';
    SET @i = 0;

    WHILE (@i < length) DO
        SET @returnStr = CONCAT(@returnStr, substring(@allowedChars, FLOOR(RAND() * LENGTH(@allowedChars) + 1), 1));
        SET @i = @i + 1;
    END WHILE;
    RETURN @returnStr;
END; //

DELIMITER //
DROP FUNCTION IF EXISTS `RandNumber`;
CREATE FUNCTION `RandNumber`(length SMALLINT(3)) RETURNS varchar(100) CHARSET utf8
begin
    SET @returnStr = '';
    SET @allowedChars = '0123456789';
    SET @i = 0;

    WHILE (@i < length) DO
        SET @returnStr = CONCAT(@returnStr, substring(@allowedChars, FLOOR(RAND() * LENGTH(@allowedChars) + 1), 1));
        SET @i = @i + 1;
    END WHILE;
    RETURN @returnStr;
END; //

DELIMITER //
DROP FUNCTION IF EXISTS `Lipsum`;
CREATE FUNCTION `Lipsum`(p_max_words SMALLINT
                       ,p_min_words SMALLINT
                       ,p_start_with_lipsum TINYINT(1)
                       )
    RETURNS VARCHAR(10000)
    NO SQL
    BEGIN
        DECLARE v_max_words SMALLINT DEFAULT 50;
        DECLARE v_random_item SMALLINT DEFAULT 0;
        DECLARE v_random_word VARCHAR(25) DEFAULT '';
        DECLARE v_start_with_lipsum TINYINT DEFAULT 0;
        DECLARE v_result VARCHAR(10000) DEFAULT '';
        DECLARE v_iter INT DEFAULT 1;
        DECLARE v_text_lipsum VARCHAR(1500) DEFAULT 'a ac accumsan ad adipiscing aenean aliquam aliquet amet ante aptent arcu at auctor augue bibendum blandit class commodo condimentum congue consectetuer consequat conubia convallis cras cubilia cum curabitur curae; cursus dapibus diam dictum dignissim dis dolor donec dui duis egestas eget eleifend elementum elit enim erat eros est et etiam eu euismod facilisi facilisis fames faucibus felis fermentum feugiat fringilla fusce gravida habitant hendrerit hymenaeos iaculis id imperdiet in inceptos integer interdum ipsum justo lacinia lacus laoreet lectus leo libero ligula litora lobortis lorem luctus maecenas magna magnis malesuada massa mattis mauris metus mi molestie mollis montes morbi mus nam nascetur natoque nec neque netus nibh nisi nisl non nonummy nostra nulla nullam nunc odio orci ornare parturient pede pellentesque penatibus per pharetra phasellus placerat porta porttitor posuere praesent pretium primis proin pulvinar purus quam quis quisque rhoncus ridiculus risus rutrum sagittis sapien scelerisque sed sem semper senectus sit sociis sociosqu sodales sollicitudin suscipit suspendisse taciti tellus tempor tempus tincidunt torquent tortor tristique turpis ullamcorper ultrices ultricies urna ut varius vehicula vel velit venenatis vestibulum vitae vivamus viverra volutpat vulputate';
        DECLARE v_text_lipsum_wordcount INT DEFAULT 180;
        DECLARE v_sentence_wordcount INT DEFAULT 0;
        DECLARE v_sentence_start BOOLEAN DEFAULT 1;
        DECLARE v_sentence_end BOOLEAN DEFAULT 0;
        DECLARE v_sentence_lenght TINYINT DEFAULT 9;

        SET v_max_words := COALESCE(p_max_words, v_max_words);
        SET v_start_with_lipsum := COALESCE(p_start_with_lipsum , v_start_with_lipsum);

        IF p_min_words IS NOT NULL THEN
            SET v_max_words := FLOOR(p_min_words + (RAND() * (v_max_words - p_min_words)));
        END IF;

        IF v_max_words < v_sentence_lenght THEN
            SET v_sentence_lenght := v_max_words;
        END IF;

        IF p_start_with_lipsum = 1 THEN
            SET v_result := CONCAT(v_result,'Lorem ipsum dolor sit amet');
            SET v_max_words := v_max_words - 5;
        END IF;

        WHILE v_iter <= v_max_words DO
            SET v_random_item := FLOOR(1 + (RAND() * v_text_lipsum_wordcount));
            SET v_random_word := REPLACE(SUBSTRING(SUBSTRING_INDEX(v_text_lipsum, ' ' ,v_random_item),
                                           CHAR_LENGTH(SUBSTRING_INDEX(v_text_lipsum,' ', v_random_item -1)) + 1),
                                           ' ', '');

            SET v_sentence_wordcount := v_sentence_wordcount + 1;
            IF v_sentence_wordcount = v_sentence_lenght THEN
                SET v_sentence_end := 1 ;
            END IF;

            IF v_sentence_start = 1 THEN
                SET v_random_word := CONCAT(UPPER(SUBSTRING(v_random_word, 1, 1))
                                            ,LOWER(SUBSTRING(v_random_word FROM 2)));
                SET v_sentence_start := 0 ;
            END IF;

            IF v_sentence_end = 1 THEN
                IF v_iter <> v_max_words THEN
                    SET v_random_word := CONCAT(v_random_word);
                END IF;
                SET v_sentence_lenght := FLOOR(9 + (RAND() * 7));
                SET v_sentence_end := 0 ;
                SET v_sentence_start := 1 ;
                SET v_sentence_wordcount := 0 ;
            END IF;

            SET v_result := CONCAT(v_result,' ', v_random_word);
            SET v_iter := v_iter + 1;
        END WHILE;

        RETURN TRIM(CONCAT(v_result));
END;
//

DELIMITER ;
INSERT INTO Addresses
        SET Number = '1305',
        Street = 'Av. Min. Gentil Barreira',
        Neighborhood = 'Sapiranga',
        City = 'Fortaleza',
        State = 'Ceará';
    INSERT INTO Users
        SET Name = 'Administrator',
        Login = 'admin',
        Password = "$2a$11$47O23iczxidkRRDQ9tbgXOMynRWnjO1kaaO5mkC4FWaiVnF4oaNN.",
        Cpf = '24.374.575/0001-32',
        Email = 'admin@trilogo.com.br',
        Birthday = '2016-03-09',
        AddressId = LAST_INSERT_ID(),
        CreationDate = '2016-03-09';

DELIMITER //
CREATE PROCEDURE PopulateUsers()
BEGIN
    DECLARE v_rep int unsigned default 50;
    DECLARE v_ite int unsigned default 1;
    DECLARE v_name varchar(100) default '';
    WHILE v_ite < v_rep DO
        SET v_name = Lipsum(1, NULL, NULL);
        INSERT INTO Addresses
            SET Number = RandNumber(3),
            Street = CONCAT('Rua', ' ', Lipsum(2, 1, NULL)),
            Neighborhood = Lipsum(2, 1, NULL),
            City = Lipsum(2, 1, NULL),
            State = Lipsum(1, NULL, NULL);
        INSERT INTO Users
            SET Name = v_name,
            Login = CONCAT(LOWER(v_name), RandNumber(3)),
            Password = RandString(16),
            Cpf = RandNumber(11),
            Email = CONCAT(LOWER(v_name), '@mail.com'),
            Birthday = CURRENT_DATE - INTERVAL 25 YEAR - INTERVAL FLOOR(RAND() * 5000) DAY,
            AddressId = LAST_INSERT_ID(),
            CreationDate = CURRENT_DATE - INTERVAL FLOOR(RAND() * 5000) DAY;
        SET v_ite = v_ite + 1;
    END WHILE;
END; //

DELIMITER ;
CALL PopulateUsers();

DELIMITER //
CREATE PROCEDURE PopulateProducts()
BEGIN
    DECLARE v_rep int unsigned default 100;
    DECLARE v_ite int unsigned default 1;
    WHILE v_ite < v_rep DO
        INSERT INTO Products
            SET Name = CONCAT(Lipsum(2, 2, NULL), ' Ref. ', UPPER(RandString(4))),
            Description = Lipsum(10, 5, NULL),
            Price = 500 * RAND() + 1,
            StockQuantity = 50 + RAND()*100;
        SET v_ite = v_ite + 1;
    END WHILE;
END; //
DELIMITER ;
CALL PopulateProducts();

DELIMITER //
CREATE PROCEDURE PopulateOrders()
BEGIN
    DECLARE v_rep int unsigned default 200;
    DECLARE v_ite int unsigned default 1;
    DECLARE v_rep2 int unsigned default 4;
    DECLARE v_ite2 int unsigned default 1;
    DECLARE v_quantity int unsigned default 50;
    DECLARE v_status int unsigned default 0;
    DECLARE v_cdate datetime(6) default CURRENT_DATE;
    DECLARE v_price decimal(65,30) default 0;
    DECLARE v_numprod int unsigned default 0;
    DECLARE v_lastid int unsigned default 0;
    DECLARE v_acum decimal(65,30) default 0;
    WHILE v_ite < v_rep DO
        SET v_cdate = CURRENT_DATE - INTERVAL FLOOR(RAND() * 3000) DAY;
        SET v_status = ROUND(RAND() + 2);
        IF v_status = 2 THEN
            INSERT INTO Orders
                SET ClientId = FLOOR(((v_quantity - 1) * RAND()) + 3),
                CreationDate = v_cdate,
                Status = v_status,
                TotalValue = 0,
                CancellationDate = v_cdate + INTERVAL FLOOR(RAND()* 7) DAY,
                FinishingDate = NULL;
        ELSE 
            INSERT INTO Orders
                SET ClientId = FLOOR(((v_quantity - 1) * RAND()) + 3),
                CreationDate = v_cdate,
                Status = v_status,
                TotalValue = 0,
                CancellationDate = NULL,
                FinishingDate = v_cdate + INTERVAL FLOOR(RAND()* 7) DAY;
        END IF;
        SET v_lastid = LAST_INSERT_ID();
        SET v_rep2 = FLOOR(4*RAND() + 2);
        WHILE v_ite2 < v_rep2 DO
            SET v_numprod = 5 * RAND() + 1;
            SET v_price = 500 * RAND() + 1;
            INSERT INTO Items
                SET ProductId = FLOOR(99 * RAND() + 1),
                    Price = v_price,
                    Quantity = v_numprod, 
                    OrderId = v_lastid;
        SET v_ite2 = v_ite2 + 1;
        SET v_acum = v_acum + v_price*v_numprod;
        END WHILE;
        UPDATE Orders
            SET TotalValue = v_acum
            WHERE Id = v_lastid;
        SET v_acum = 0;
        SET v_ite = v_ite + 1;
        SET v_ite2 = 1;
    END WHILE;
END; //
DELIMITER ;
CALL PopulateOrders();
