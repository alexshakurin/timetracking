create table timetracking
(
	id integer primary key autoincrement,
	date text not null,
	minutes integer not null,
	memo text null,
	sync int not null
)