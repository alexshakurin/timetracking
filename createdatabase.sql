create table timetracking
(
	date text not null,
	seconds real not null,
	latestmemo text null,
	primary key(date)
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

create table workingtimeintervals
(
	aggregateid text not null,
	date text not null,
	starttime text not null,
	endtime text not null,
	memo text not null
)

