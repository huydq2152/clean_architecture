﻿using Contracts.Domains.Interfaces;

namespace Contracts.Domains;

public class FullAuditedEntityBase<T> : EntityBase<T>, IFullAuditedEntityBase
{
    public int? CreatorUserId { get; set; }
    public DateTime CreationTime { get; set; }
    public int? LastModifiedUserId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int? DeleterUserId { get; set; }
    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }
}