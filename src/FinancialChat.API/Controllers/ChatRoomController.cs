using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.DTOs.Common;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialChat.API.Controllers;

[ApiController]
[Route("api/chatrooms")]
[Authorize]
public class ChatRoomController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ChatRoomController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ChatRoomDto>>>> GetAll()
    {
        var chatRooms = await _serviceManager.ChatRoomService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ChatRoomDto>>.SuccessResponse(chatRooms));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ChatRoomDto>>> GetById(Guid id)
    {
        var chatRoom = await _serviceManager.ChatRoomService.GetByIdAsync(id);
        return Ok(ApiResponse<ChatRoomDto>.SuccessResponse(chatRoom));
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<PagedResult<MessageDto>>>> GetMessages(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _serviceManager.ChatRoomService.GetMessagesAsync(id, page, pageSize);
        return Ok(ApiResponse<PagedResult<MessageDto>>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ChatRoomDto>>> Create([FromBody] CreateChatRoomDto dto)
    {
        var chatRoom = await _serviceManager.ChatRoomService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = chatRoom.Id },
            ApiResponse<ChatRoomDto>.SuccessResponse(chatRoom, "Chat room created successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        await _serviceManager.ChatRoomService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse("Chat room deleted successfully"));
    }
}
