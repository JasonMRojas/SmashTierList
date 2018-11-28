USE master;


IF EXISTS(select * from sys.databases where name='Tier_List')
DROP DATABASE Tier_List;


CREATE DATABASE Tier_List;


USE Tier_List;


BEGIN TRANSACTION;
CREATE TABLE tier_list
(
	id			int			identity(1,1),
	name		varchar(50)	null,
	
	constraint pk_tier_list	primary key(id)
);

CREATE TABLE tier_rows
(
	id					int				identity(1,1),
	name				varchar(50)		not null,
	tier_list_id		int				not null,
	order_value			int				null,

	constraint pk_tier_rows primary key (id),
	constraint fk_tier_list_rows foreign key (tier_list_id) references tier_list(id)
);

CREATE TABLE items
(
	id					int				identity(1,1),
	image_path			varchar(500)	not null,
	order_value			int				not null,
	row_id				int				not null,

	constraint pk_image_paths primary key (id),
	constraint fk_image_row foreign key (row_id) REFERENCES tier_rows(id)
);

COMMIT TRANSACTION;