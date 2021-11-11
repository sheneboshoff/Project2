CREATE PROCEDURE [dbo].[PhotoAddOrEdit]
	@PhotoId int,
	@UserId nvarchar(450),
	@Photo_Name varchar(100),
	@Photo_Format varchar(100),
	@Photo_Geolocation varchar(100),
	@Photo_Tags varchar(100),
	@Photo_CaptureDate date
AS
BEGIN
	SET NOCOUNT ON;

	if @PhotoId = 0
	begin
		insert into Photo(UserId, Photo_Name, Photo_Format, Photo_Geolocation, Photo_Tags, Photo_CaptureDate)
		values(@UserId, @Photo_Name, @Photo_Format, @Photo_Geolocation, @Photo_Tags, @Photo_CaptureDate)
	end
	else
	begin
		update Photo
		set
			Photo_Geolocation = @Photo_Geolocation,
			Photo_Tags = @Photo_Tags,
			Photo_CaptureDate = @Photo_CaptureDate
		where PhotoId = @PhotoId
	end
end
go
