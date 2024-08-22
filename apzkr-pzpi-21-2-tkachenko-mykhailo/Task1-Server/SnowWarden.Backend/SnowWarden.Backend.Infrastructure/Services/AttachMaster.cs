using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using SnowWarden.Backend.Core;
using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Utils.Comparers;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;
using SnowWarden.Backend.Core.Utils.Reflection;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Exceptions;

namespace SnowWarden.Backend.Infrastructure.Services;

public class AttachMaster
{
	private readonly ApplicationDbContext _context;

	public AttachMaster(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task Attach<TPrincipal, TRelation>(
		TPrincipal source,
		TPrincipal? compare,
		Expression<Func<TPrincipal, ICollection<TRelation>>> relationSelector,
		bool withCreate = false)
		where TPrincipal : class, IDbEntity
		where TRelation : class, IDbEntity
	{
		string path = relationSelector.GetPropertyPath();
		(List<TRelation> Detach, List<TRelation> Attach) changes = DetectChanges<TPrincipal, TRelation>(source, compare, path);

		await AttachInternal(source, changes.Attach, path, withCreate);
	}

	private async Task AttachInternal<TPrincipal, TRelation>(
		TPrincipal source,
		ICollection<TRelation> attach, string propertyPath, bool withCreate)
		where TPrincipal : class, IDbEntity
		where TRelation : class, IDbEntity
	{
		ICollection<TRelation> sourceValues = source.GetType().GetProperty(propertyPath)?.GetValue(source) as ICollection<TRelation> ?? [];
		List<TRelation> list = sourceValues.ToList();
		List<int> attachIds = attach.Select(a => a.Id).ToList();
		List<TRelation> existingAttaches = await _context.Set<TRelation>().Where(r => attachIds.Contains(r.Id)).ToListAsync();

		foreach (TRelation toAttach in attach)
		{
			TRelation? existingAttach = existingAttaches.FirstOrDefault(r => r.Id == toAttach.Id);

			switch (withCreate)
			{
				case false when existingAttach is null:
					throw new InvalidAttachmentEntityException(toAttach);
				case true when existingAttach is null:
					_context.Set<TRelation>().Add(toAttach);
					break;
			}

			if (list.Contains(toAttach, new DbEntityComparer<TRelation>()))
				list.Remove(list.First(r => r.Id == toAttach.Id));
			list.Add(existingAttach ?? toAttach);
		}

		source.GetType().GetProperty(propertyPath)?.SetValue(source, list);
	}

	public async Task Detach<TPrincipal, TRelation>(
		TPrincipal source,
		TPrincipal compare,
		Expression<Func<TPrincipal, ICollection<TRelation>>> relationSelector,
		bool withDelete = false)
		where TPrincipal : class, IDbEntity
		where TRelation : class, IDbEntity
	{
		string path = relationSelector.GetPropertyPath();
		(List<TRelation> Detach, List<TRelation> Attach) changes = DetectChanges<TPrincipal, TRelation>(source, compare, path);

		await DetachInternal(source, changes.Detach, path, withDelete);
	}

	private (List<TRelation> Detach, List<TRelation> Attach) DetectChanges<TPrincipal, TRelation>(TPrincipal source, TPrincipal? compare, string path)
		where TPrincipal : class, IDbEntity
		where TRelation : class, IDbEntity
	{
		ICollection<TRelation> sourceCollection = source.GetType().GetProperty(path)?.GetValue(source) as ICollection<TRelation> ?? [];
		ICollection<TRelation> compareCollection = compare?.GetType().GetProperty(path)?.GetValue(compare) as ICollection<TRelation> ?? [];
		List<TRelation> toDetach = sourceCollection.Except(compareCollection, new DbEntityComparer<TRelation>()).ToList();
		List<TRelation> toAttach = compare is null
			? sourceCollection.ToList()
			: compareCollection
				.Except(sourceCollection, new DbEntityComparer<TRelation>())
				.ToList();

		return (toDetach, toAttach);
	}

	private async Task DetachInternal<TPrincipal, TRelation>(
		TPrincipal source,
		ICollection<TRelation> detach,
		string propertyPath,
		bool withDelete)
		where TPrincipal : class, IDbEntity
		where TRelation : class, IDbEntity
	{
		ICollection<TRelation> sourceValues = source.GetType().GetProperty(propertyPath)?.GetValue(source) as ICollection<TRelation> ?? [];
		foreach (TRelation toDetach in detach)
		{
			TRelation svToDetach = sourceValues.FirstOrDefault(sv => sv.Id == toDetach.Id)
				?? throw new InvalidAttachmentEntityException(toDetach);
			sourceValues.Remove(svToDetach);

			if (!withDelete) continue;

			TRelation toRemove = await _context.Set<TRelation>().FindAsync(svToDetach.Id)
				?? throw new InfrastructureException(new LocalizedContent
				{
					Translations = new LocalizationDictionary
					{
						{
							Localizator.SupportedLanguages.AmericanEnglish,
							$"Detachment of {typeof(TRelation)} with id {toDetach.Id} from {typeof(TPrincipal)} has failed"
						},
						{
							Localizator.SupportedLanguages.Ukrainian,
							$"Від'єднання вкладеної сутності {typeof(TRelation)} з ідентифікатором {toDetach.Id} з {typeof(TPrincipal)} провалилось"
						}
					}
				});
			_context.Set<TRelation>().Remove(toRemove);
		}
	}
}