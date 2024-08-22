using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Requests.Dtos;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Models.Response.Dtos;

using SnowWarden.Backend.Core.Exceptions;

using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Inventory.Services;
using SnowWarden.Backend.Core.Features.Members;

using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;
using SnowWarden.Backend.Core.Utils.Localization.Services;

namespace SnowWarden.Backend.API.Controllers.Backoffice;

[Route("backoffice/inventories")]
public class BackofficeInventoryController(
	IInventoryService inventoryService,
	IInventoryItemService inventoryItemService,
	IMapper mapper) : ControllerBase
{
	[Route("")]
	[HttpGet]
	[Authorize(Roles =
		$"{Admin.ROLE_NAME}," +
		$"{Instructor.ROLE_NAME}")]
	public async Task<IActionResult> Get()
	{
		IReadOnlyCollection<Inventory> inventories = await inventoryService.GetReadonlyLightweightAsync();
		IEnumerable<InventoryResponseDto> mapped = inventories.Select(mapper.Map<Inventory, InventoryResponseDto>);

		return inventories.Any()

			? ResponseObject<IEnumerable<InventoryResponseDto>>
				.Status200Ok(mapped)
				.ToObjectResult()

			: ResponseObject<IReadOnlyCollection<InventoryResponseDto>>
				.Status204NoContent<Inventory>()
				.ToObjectResult();
	}

	[Route("{id:int}")]
	[HttpGet]
	[Authorize(Roles =
		$"{Admin.ROLE_NAME}," +
		$"{Instructor.ROLE_NAME}")]
	public async Task<IActionResult> GetById([FromRoute] int id)
	{
		Inventory? inventory = await inventoryService.GetReadonlyByIdCompleteAsync(id);

		if (inventory is null)
		{
			return ResponseObject<Inventory>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		InventoryResponseDto response = mapper.Map<Inventory, InventoryResponseDto>(inventory);
		return ResponseObject<InventoryResponseDto>
			.Status200Ok(response)
			.ToObjectResult();
	}

	[Route("")]
	[HttpPost]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> CreateInventory([FromBody] InventoryPostDto request)
	{
		try
		{
			Inventory? mapped = mapper.Map<InventoryPostDto, Inventory>(request);
			Inventory created = await inventoryService.CreateAsync(mapped);
			Inventory? read = await inventoryService.GetReadonlyByIdCompleteAsync(created.Id);
			InventoryResponseDto response = mapper.Map<Inventory, InventoryResponseDto>(read);
			return ResponseObject<InventoryResponseDto>
				.Status201Created(response)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Inventory>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}/items")]
	[HttpPut]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> AddItem(
		[FromRoute] int id,
		[FromBody] InventoryItemPostDto request,
		[FromQuery] int amount = 1)
	{
		Inventory? inventory = await inventoryService.GetByIdCompleteAsync(id);
		if (inventory is null)
		{
			return ResponseObject<Inventory>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}
		try
		{
			List<InventoryItem> items = [];
			for (int i = 0; i < amount; i++)
			{
				InventoryItem? mapped = mapper.Map<InventoryItemPostDto, InventoryItem>(request);
				mapped.Attributes = mapped.Attributes?.Select(attr => new InventoryAttribute
				{
					Title = attr.Title,
					Value = attr.Value
				}).ToList();
				mapped.Inventory = inventory;
				items.Add(mapped);
			}

			ICollection<InventoryItem> created = await inventoryItemService.CreateRange(items.ToArray());

			return ResponseObject<IEnumerable<InventoryItemResponseDto>>
				.Status201Created(created.Select(mapper.Map<InventoryItem, InventoryItemResponseDto>))
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Inventory>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}/items/{itemId:int}")]
	[HttpDelete]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> RemoveItem(
		[FromRoute] int id,
		[FromRoute] int itemId)
	{
		Inventory? inventory = await inventoryService.GetReadonlyByIdCompleteAsync(id);
		if (inventory is null)
		{
			return ResponseObject<Inventory>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		InventoryItem? item = await inventoryItemService.GetReadonlyByIdCompleteAsync(itemId);
		if (item is null)
		{
			return ResponseObject<InventoryItem>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		try
		{
			InventoryItem deleted = await inventoryItemService.DeleteByIdAsync(item.Id);
			LocalizedContent responseMessage = new()
			{
				Translations = new LocalizationDictionary
				{
					{ Localizator.SupportedLanguages.AmericanEnglish, $"{nameof(InventoryItem)} {deleted.Name} has been deleted successfully" },
					{ Localizator.SupportedLanguages.Ukrainian, $"{nameof(InventoryItem)} {deleted.Name} було успішно видалено" }
				}
			};

			return ResponseObject<string>
				.Status200Ok(responseMessage.ToLocalizedString(User.Language()))
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Inventory>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}/items/{itemId:int}")]
	[HttpPatch]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> UpdateItem(
		[FromRoute] int id,
		[FromRoute] int itemId,
		[FromBody] InventoryItemPostDto request)
	{
		Inventory? inventory = await inventoryService.GetByIdCompleteAsync(id);
		if (inventory is null)
		{
			return ResponseObject<Inventory>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		InventoryItem? inventoryItem = mapper.Map<InventoryItemPostDto, InventoryItem>(request);
		inventoryItem.SetExistingId(itemId);
		inventoryItem.Inventory = inventory;
		InventoryItem? dbItem = await inventoryItemService.GetReadonlyByIdCompleteAsync(inventoryItem.Id);
		if (dbItem is null)
		{
			return ResponseObject<InventoryItem>
				.ResourceNotFoundResult(inventoryItem.Id, User.Language())
				.ToObjectResult();
		}

		try
		{
			InventoryItem updated = await inventoryItemService.UpdateAsync(inventoryItem);
			InventoryItem? read = await inventoryItemService.GetReadonlyByIdCompleteAsync(updated.Id);
			InventoryItemResponseDto response = mapper.Map<InventoryItem, InventoryItemResponseDto>(read);
			return ResponseObject<InventoryItemResponseDto>
				.Status200Ok(response)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Inventory>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}
}