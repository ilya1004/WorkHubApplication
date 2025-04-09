export interface PaginatedResult<TValue> {
  items: TValue[];
  totalCount: number;
  pageNo: number;
  pageSize: number;
}
