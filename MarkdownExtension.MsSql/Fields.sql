SELECT
    COLUMN_NAME as [Name],
    DATA_TYPE as [Type],
    case IS_NULLABLE 
		when 'YES' then 1
		when 'NO' then 0
	end
	as [IsNullable],
    CHARACTER_MAXIMUM_LENGTH as [MaxLength],
    NUMERIC_PRECISION as [NumericPrecision],
    NUMERIC_SCALE as [NumericScale]
FROM Test.INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = '{0}' AND
    COLUMN_NAME <> 'principal_id' AND
    COLUMN_NAME <> 'name' AND
    COLUMN_NAME <> 'definition' AND
    COLUMN_NAME <> 'diagram_id' AND
    COLUMN_NAME <> 'version'
ORDER BY ORDINAL_POSITION