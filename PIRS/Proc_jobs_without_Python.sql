use PIRS
go

declare @table_id int
declare @id_record int
declare @id_main_table int;

declare @idd int
set @idd = 12200
while @idd < 13000
begin
select @id_record = id_record, @table_id = table_id from dbo.jobs where (id_record = @idd)
set @id_main_table = 885578193 --(select table_id from dbo.used_tables where id = 1);
if @table_id = 885578193 --@id_main_table
begin
	declare @desc nvarchar (1000)

	select 
		@desc = description 
	from 
		dbo.main_table 
	where 
		id = @id_record

	select 
		@desc = dbo.delete_after_sleshes(@desc)
	update 
		dbo.jobs 
	set 
		status = 1, 
		ts_finish = getdate() 
	where 
		id_record = @id_record

	exec dbo.clean_fio_str @desc = @desc, @out = @desc output

	update 
		dbo.jobs 
	set 
		status = 2, 
		ts_finish = getdate() 
	where id_record = @id_record

	select 
		@desc = dbo.clear_with_reg_expr(@desc)
	update 
		dbo.jobs 
	set 
		status = 3, 
		ts_finish = getdate() 
	where 
		id_record = @id_record

	create table #vrem(word nvarchar(200))

	insert 
		#vrem (word) 
	select 
		value 
	from 
		dbo.uf_inline_split_me_pirs(@desc,'')

	update 
		dbo.jobs 
	set 
		status = 4, 
		ts_finish = getdate() 
	where 
		id_record = @id_record

	declare @id_author int
	select 
		@id_author = a.id 
	from 
		dbo.authors a 
	inner join
		dbo.main_table b 
	on 
		a.author_fio = b.author 
		and 
		b.id = @id_record

	declare @id_word int;

	DECLARE @vword nvarchar(1000);
		declare word_curs CURSOR FOR SELECT word from #vrem;
		open word_curs;
		FETCH NEXT FROM word_curs INTO @vword; 
		WHILE @@fetch_status = 0
		BEGIN		
		if (len(@vword)>3)
		begin
			select @id_word = id 
			FROM dbo.dict 
			WHERE word_nf = @vword;

			if (@id_word is NULL)
				begin

				insert into 
					dbo.dict 
					(word_nf) 
				select 
					@vword;

				set @id_word= (select IDENT_CURRENT('dict'));
				end

			insert into 
				dbo.words_step2
				(id_record, 
				id_table, 
				id_word_nf, 
				id_author)
			select 
				@id_record, 
				@table_id, 
				@id_word,  
				@id_author

			set @id_word = NULL
		end
		FETCH NEXT FROM word_curs INTO @vword; 
		END
		CLOSE word_curs;
		DEALLOCATE word_curs;
	
	drop table if exists #vrem;
	drop table if exists #vrem2;
	update 
		dbo.jobs 
	set 
		status = 5, 
		ts_finish = getdate() 
	where 
		id_record = @id_record
end

set @idd = @idd+1
end
