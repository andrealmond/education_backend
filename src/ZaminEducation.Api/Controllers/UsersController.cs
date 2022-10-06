using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZaminEducation.Api.Extensions;
using ZaminEducation.Api.Extensions.Attributes;
using ZaminEducation.Domain.Configurations;
using ZaminEducation.Domain.Entities.UserCourses;
using ZaminEducation.Domain.Entities.Users;
using ZaminEducation.Service.DTOs.UserCourses;
using ZaminEducation.Service.DTOs.Users;
using ZaminEducation.Service.Interfaces;

namespace ZaminEducation.Api.Controllers;

public class UsersController : BaseController
{
    private readonly IUserService userService;
    private readonly ISavedCoursesService savedCoursesService;
    public UsersController(IUserService userService, ISavedCoursesService savedCoursesService)
    {
        this.userService = userService;
        this.savedCoursesService = savedCoursesService;
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="dto">user creating initial info taker dto</param>
    /// <returns>Created user infortaions</returns>
    /// <response code="200">If user is created successfully</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<User>> CreateAsync(UserForCreationDto dto) =>
        Ok(await userService.CreateAsync(dto));

    /// <summary>
    /// Toggle saved course
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("saved-course")]
    public async ValueTask<ActionResult<SavedCourse>> ToggleAsync(SavedCourseForCreationDto dto) =>
        Ok(await savedCoursesService.ToggleAsync(dto));

    /// <summary>
    /// delete user by id (for only admins)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if user deleted succesfully else false</returns>
    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async ValueTask<ActionResult<bool>> DeleteAsync([FromRoute] long id) =>
        Ok(await userService.DeleteAsync(user => user.Id == id));


    /// <summary>
    /// get all of users (for only admins)
    /// </summary>
    /// <param name="params">pagenation params</param>
    /// <returns> user collection </returns>
    [HttpGet, Authorize(Roles = "Admin")]
    public async ValueTask<ActionResult<IEnumerable<User>>> GetAllAsync(
        [FromQuery] PaginationParams @params) =>
            Ok(await userService.GetAllAsync(@params));

    /// <summary>
    /// Get all saved courses of users
    /// </summary>
    /// <param name="params"></param>
    /// <returns></returns>
    [HttpGet("saved-course")]
    public async ValueTask<ActionResult<IEnumerable<SavedCourse>>> GetAllSavedCoursesAsync(
        [FromQuery] PaginationParams @params) =>
            Ok(await savedCoursesService.GetAllAsync(@params));


    [HttpPost("Change/Password"), Authorize(Policy = "AllPolicy")]
    public async ValueTask<ActionResult<User>> ChangePasswordAsync(UserForChangePassword dto) =>
        Ok(await userService.ChangePasswordAsync(dto));

    /// <summary>
    /// get one user information by id
    /// </summary>
    /// <param name="id">user id</param>
    /// <returns>user</returns>
    /// <response code="400">if user data is not in the base</response>
    /// <response code="200">if user data have in database</response>
    [HttpGet("{id}"), Authorize("AllPolicy")]
    public async ValueTask<ActionResult<User>> GetAsync([FromRoute]long id) =>
        Ok(await userService.GetAsync(user => user.Id == id));

    /// <summary>
    /// update user 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut, Authorize("AllPolicy")]
    public async ValueTask<ActionResult<User>> UpdateAsync(
        long id, [FromBody] UserForCreationDto dto) =>
            Ok(await userService.UpdateAsync(id, dto));

    /// <summary>
    /// get self user info without any id
    /// </summary>
    /// <returns>user</returns>
    [HttpGet("info"), Authorize]
    public async ValueTask<ActionResult<User>> GetInfoAsync()
        => Ok(await userService.GetInfoAsync());

    /// <summary>
    /// create attachment for user for all id
    /// </summary>
    /// <returns></returns>
    [HttpPost("attachments/{id}"), Authorize("UserPolicy")]
    public async Task<IActionResult> Attachment(long id, [FormFileAttributes, IsNoMoreThenMaxSize(3145728)] IFormFile formFile)
        => Ok(await userService.AddAttachmentAsync(id, formFile.ToAttachmentOrDefault()));
}

