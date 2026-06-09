alter table reports
    add column if not exists completionstatus varchar(100) not null default '';

alter table reports
    add column if not exists formedat timestamp without time zone;

update reports
set formedat = createdat
where formedat is null;

alter table reports
    alter column formedat set not null;

create index if not exists ix_reports_taskid
    on reports (taskid);

create index if not exists ix_reports_userid
    on reports (userid);

create index if not exists ix_reportphotos_reportid
    on reportphotos (reportid);
