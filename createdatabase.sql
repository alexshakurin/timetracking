create table timetracking
(
	id integer primary key autoincrement,
	date text not null,
	minutes integer not null,
	memo text null,
	sync int not null
)

create table storedevents
(
	aggregateid text not null,
	aggregatetype text not null,
	version integer not null,
	payload text not null,
	correlationid text not null,
	primary key (aggregateid, aggregatetype, version)
)