CREATE TABLE mark
(
    id        integer not null
        constraint mark_pk
            primary key autoincrement,
    name      text    not null,
    full_name text    not null
);

CREATE TABLE project
(
    id     integer not null
        constraint project_pk
            primary key autoincrement,
    cipher text    not null,
    name   text    not null
);

CREATE TABLE design_object
(
    id               integer not null
        constraint design_object_pk
            primary key autoincrement,
    code             text    not null,
    name             text    not null,
    project_id       integer not null,
    design_object_id integer,

    constraint design_object_design_object_fk
        foreign key (design_object_id)
            references design_object (id)
            on update cascade
            on delete restrict,
    constraint design_object_project_fk
        foreign key (project_id)
            references project
            on update cascade
            on delete restrict
);

CREATE TABLE documentation_set
(
    id               integer not null
        constraint documentation_set_pk
            primary key autoincrement,
    mark_id          integer not null,
    number           integer not null,
    design_object_id integer not null,
    constraint documentation_set_mark_fk
        foreign key (mark_id)
            references mark (id)
            on update cascade
            on delete restrict,
    constraint documentation_set_design_object_fk
        foreign key (design_object_id)
            references design_object (id)
            on update cascade
            on delete restrict
);

INSERT INTO mark (id, name, full_name)
VALUES (1, 'ТХ', 'Технология производства');
INSERT INTO mark (id, name, full_name)
VALUES (2, 'АС', 'Архитектурно-строительные решения');
INSERT INTO mark (id, name, full_name)
VALUES (3, 'СМ', 'Сметная документация');

INSERT INTO project (id, cipher, name)
VALUES (1, 'ГЗУ0618-С932-Стр', 'Строительство нефтепровода Скв.932 – ГЗУ-0618 Батырбайского месторождения');
INSERT INTO project (id, cipher, name)
VALUES (2, 'ГЗУ0618-С932-Ре1', 'Реконструкция нефтепровода Скв.932 – ГЗУ-0618 Батырбайского месторождения');

INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (1, 'ВТбп', 'Выкидной трубопровод', null, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (2, 'С932', 'Скважина №932', null, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (3, 'Са', 'Сальник', 1, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (4, 'Тр', 'Тройник', 1, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (5, 'УСШ', 'Устьевой сальниковый шток', 1, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (6, 'ТГ', 'Трубная головка', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (7, 'ЦС', 'Цементный стакан', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (8, 'Н', 'Направление', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (9, 'К', 'Кондуктор', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (10, 'ВПТС', 'ВПТС', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (11, 'ПК', 'Промежуточная колонна', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (12, 'ЭК', 'Эксплутационная колонна', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (13, 'ПП', 'Продуктивный пласт', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (14, 'ТУ', 'Телескопическое устройство', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (15, 'З', 'Забой', 2, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (16, 'ЭЦН', 'ЭЦН', 15, 2);
INSERT INTO design_object (id, code, name, design_object_id, project_id)
VALUES (17, 'ОК', 'Обсадная колонна', 15, 2);